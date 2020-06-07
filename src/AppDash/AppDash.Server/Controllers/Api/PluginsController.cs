using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Plugins;
using AppDash.Plugins.Controllers;
using AppDash.Server.Plugins;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using PluginManager = AppDash.Server.Plugins.PluginManager;

namespace AppDash.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PluginsController : ControllerBase
    {
        private readonly PluginManager _pluginManager;
        private readonly PluginControllerMatcher _pluginControllerMatcher;
        private readonly PluginSettingsManager _pluginSettingsManager;

        public PluginsController(PluginManager pluginManager, PluginControllerMatcher pluginControllerMatcher, PluginSettingsManager pluginSettingsManager)
        {
            _pluginManager = pluginManager;
            _pluginControllerMatcher = pluginControllerMatcher;
            _pluginSettingsManager = pluginSettingsManager;
        }

        [HttpGet]
        public ApiResult Get()
        {
            var plugins = _pluginManager.GetPlugins();

            return ApiResult.Success(plugins.Select(plugin =>
            {
                var icon = string.IsNullOrEmpty(plugin.PluginInstance.Icon) ? null : new
                {
                    url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/plugins/{plugin.PluginInstance.Key}/icon",
                    filename = plugin.PluginInstance.Icon
                };

                return new
                {
                    name = plugin.PluginInstance.GetType().Name,
                    key = plugin.PluginInstance.Key,
                    cssFile = plugin.CssFileUrl,
                    jsFile = plugin.JsFileUrl,
                    icon,
                    assembly = new
                    {
                        url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/plugins/{plugin.PluginInstance.GetType().Assembly.GetName().Name}.dll",
                        filename = plugin.PluginInstance.GetType().Assembly.GetName().Name + ".dll"
                    }
                };
            }));
        }

        [HttpGet]
        [Route("{pluginKey}/settings")]
        public ApiResult GetSettings(string pluginKey)
        {
            var settings = _pluginSettingsManager.GetPluginSettings(pluginKey);

            if(settings == null)
                return ApiResult.BadRequest();

            return ApiResult.Success(settings);
        }

        [HttpPatch]
        [Route("{pluginKey}/settings")]
        public ApiResult SetSettings(string pluginKey, [FromBody] PluginData pluginSettings)
        {
            bool result = _pluginSettingsManager.SetPluginSettings(pluginKey, pluginSettings);

            if (!result)
                return ApiResult.BadRequest();

            return ApiResult.NoContent();
        }

        [HttpGet]
        [HttpPost]
        [HttpPatch]
        [HttpDelete]
        [HttpPut]
        [HttpHead]
        [HttpOptions]
        [Route("{pluginKey}/{*route}")]
        public IActionResult ExecutePluginAction(string pluginKey, string route)
        {
            route = "/" + route;

            if (_pluginControllerMatcher.TryMatch(pluginKey, route, out (IPluginController, MethodInfo) pluginController))
            {
                return _pluginControllerMatcher.Execute(pluginKey, route, Request.Body, pluginController.Item1, pluginController.Item2);
            }

            return ApiResult.NotFound();
        }
    }
}

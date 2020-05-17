using System;
using System.Linq;
using AppDash.Core;
using AppDash.Plugins;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PluginManager = AppDash.Server.Plugins.PluginManager;

namespace AppDash.Server.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class PluginsController : ControllerBase
    {
        private readonly PluginManager _pluginManager;

        public PluginsController(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        [HttpGet]
        public ApiResult Get()
        {
            var plugins = _pluginManager.GetPlugins();

            return ApiResult.Success(plugins.Select(plugin =>
            {
                var icon = string.IsNullOrEmpty(plugin.Icon) ? null : new
                {
                    url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/plugins/{plugin.Key}/icon",
                    filename = plugin.Icon
                };

                return new
                {
                    name = plugin.GetType().Name,
                    key = plugin.Key,
                    icon,
                    assembly = new
                    {
                        url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/plugins/{plugin.GetType().Assembly.GetName().Name}.dll",
                        filename = plugin.GetType().Assembly.GetName().Name + ".dll"
                    }
                };
            }));
        }

        [HttpGet]
        [Route("{pluginKey}/settings")]
        public ApiResult GetSettings(string pluginKey)
        {
            var settings = _pluginManager.GetPluginSettings(pluginKey);

            Console.WriteLine(JsonConvert.SerializeObject(settings));

            if(settings == null)
                return ApiResult.BadRequest();

            return ApiResult.Success(settings);
        }

        [HttpPatch]
        [Route("{pluginKey}/settings")]
        public ApiResult SetSettings(string pluginKey, [FromBody] PluginData pluginSettings)
        {
            _pluginManager.SetPluginSettings(pluginKey, pluginSettings);

            return ApiResult.NoContent();
        }
    }
}

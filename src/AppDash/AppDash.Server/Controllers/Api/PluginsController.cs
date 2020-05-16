using System.Linq;
using AppDash.Core;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
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
    }
}

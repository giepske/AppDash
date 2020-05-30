using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppDash.Client.Plugins;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using AppDash.Server.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using PluginManager = AppDash.Server.Plugins.PluginManager;

namespace AppDash.Server.Controllers
{
    [Controller]
    [Route("[controller]")]
    public class PluginsController : ControllerBase
    {
        private readonly PluginManager _pluginManager;
        private readonly FileMemoryService _fileMemoryService;

        public PluginsController(PluginManager pluginManager, FileMemoryService fileMemoryService)
        {
            _pluginManager = pluginManager;
            _fileMemoryService = fileMemoryService;
        }

        /// <summary>
        /// Get a direct assembly by its filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{fileName}")]
        public ActionResult Get(string fileName)
        {
            if (_fileMemoryService.CheckPluginFile(fileName))
                return new FileContentResult(_fileMemoryService.GetPluginFile(fileName), "application/octet-stream");

            return new NotFoundResult();
        }

        /// <summary>
        /// Get a direct icon of a plugin by its plugin key.
        /// </summary>
        /// <param name="pluginKey"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{pluginKey}/icon")]
        public ActionResult GetIcon(string pluginKey)
        {
            var plugin = _pluginManager.GetPlugin<AppDashPlugin>(pluginKey);

            if(plugin == null || string.IsNullOrEmpty(plugin.Icon))
                return new NotFoundResult();

            if (_fileMemoryService.CheckIconFile(plugin.Key, plugin.GetType().Assembly.GetName().Name, plugin.Icon))
            {
                if (!new FileExtensionContentTypeProvider().TryGetContentType(plugin.Icon, out var contentType))
                    contentType = "application/octet-stream";

                return new FileContentResult(_fileMemoryService.GetPluginIcon(plugin.Key), contentType);
            }

            return new NotFoundResult();
        }

        [HttpGet]
        public IEnumerable<byte[]> Get()
        {
            //yield return System.IO.File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "netstandard.dll"));
            //yield return System.IO.File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Newtonsoft.Json.dll"));
            //yield return System.IO.File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "AppDash.Plugins.dll"));

            foreach (string plugin in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "plugins")))
            {
                yield return System.IO.File.ReadAllBytes(plugin);
            }
            
            //return _pluginManager.GetPlugins();
        }
    }
}

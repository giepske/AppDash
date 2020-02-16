using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Server.Plugins;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace AppDash.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PluginsController : ControllerBase
    {
        private readonly PluginManager _pluginManager;

        public PluginsController(PluginManager pluginManager)
        {
            _pluginManager = pluginManager;
        }

        //[HttpGet]
        //public FileContentResult Get()
        //{
        //    return File(System.IO.File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "AppDash.Plugins.dll")),
        //        "application/octet-stream");
        //}

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

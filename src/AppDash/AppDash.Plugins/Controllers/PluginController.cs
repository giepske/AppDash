using Microsoft.AspNetCore.Mvc;

namespace AppDash.Plugins.Controllers
{
    public class PluginController<TPlugin> : IPluginController where TPlugin : AppDashPlugin
    {
        public string PluginKey { get; set; }
    }
}

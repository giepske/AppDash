using System;
using Microsoft.AspNetCore.Mvc;

namespace AppDash.Plugins.Controllers
{

    [AttributeUsage(AttributeTargets.Method)]
    public class PluginControllerRouteAttribute : Attribute
    {
        public PluginControllerRouteAttribute(string template)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
        }

        public string Template { get; }
    }

    public interface IPluginController
    {
        string PluginKey { get; set; }
    }

    public class PluginController<TPlugin> : IPluginController where TPlugin : AppDashPlugin
    {
        public string PluginKey { get; set; }
    }
}

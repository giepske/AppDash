using System;

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
}
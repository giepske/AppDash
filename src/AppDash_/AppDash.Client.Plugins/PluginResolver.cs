using System;
using System.Collections.Generic;
using AppDash.Plugins;

namespace AppDash.Client.Plugins
{
    public class PluginResolver
    {
        private Dictionary<string, AppDashPlugin> _plugins;

        public PluginResolver()
        {
            _plugins = new Dictionary<string, AppDashPlugin>();
        }

        public AppDashPlugin AddPlugin(Type pluginType)
        {
            var pluginInstance = (AppDashPlugin)Activator.CreateInstance(pluginType);

            _plugins.Add(pluginType.FullName, pluginInstance);

            return pluginInstance;
        }

        public Dictionary<string, AppDashPlugin> GetPlugins()
        {
            return _plugins;
        }

        public AppDashPlugin GetPlugin(string key)
        {
            return _plugins[key];
        }

        public void ClearPlugins()
        {
            _plugins.Clear();
        }
    }
}
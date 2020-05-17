using System;
using System.Collections.Generic;
using System.Linq;
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

            _plugins[pluginType.FullName] = pluginInstance;

            return pluginInstance;
        }

        public Dictionary<string, AppDashPlugin> GetPlugins()
        {
            return _plugins;
        }

        public AppDashPlugin GetPlugin(string pluginKey)
        {
            return _plugins.Values.FirstOrDefault(plugin => plugin.Key == pluginKey);
        }

        public void ClearPlugins()
        {
            _plugins.Clear();
        }

        public AppDashPlugin GetPlugin(Type pluginType)
        {
            return _plugins.Values.FirstOrDefault(plugin => plugin.GetType() == pluginType);
        }
    }
}
using System;
using System.Collections.Generic;

namespace AppDash.Server.Plugins
{
    public class PluginResolver
    {
        private Dictionary<string, Type> _plugins;

        public PluginResolver()
        {
            _plugins = new Dictionary<string, Type>();
        }

        public Dictionary<string, Type> GetPlugins()
        {
            return _plugins;
        }

        public Type GetPlugin(string key)
        {
            return _plugins[key];
        }

        public void ClearPlugins()
        {
            _plugins.Clear();
        }

        public void AddPlugins(IEnumerable<Type> pluginTypes)
        {
            foreach (Type pluginType in pluginTypes)
            {
                _plugins.Add(Guid.NewGuid().ToString("N"), pluginType);
            }
        }
    }
}
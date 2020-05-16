using System;
using System.Collections.Generic;

namespace AppDash.Server.Plugins
{
    public class PluginResolver
    {
        private Dictionary<string, Type> _plugins;
        private Dictionary<Type, List<Type>> _pluginTiles;

        public PluginResolver()
        {
            _plugins = new Dictionary<string, Type>();
            _pluginTiles = new Dictionary<Type, List<Type>>();
        }

        public string AddPlugin(Type pluginType)
        {
            var guid = Guid.NewGuid().ToString("N");

            _plugins.Add(guid, pluginType);
            _pluginTiles[pluginType] = new List<Type>();

            return guid;
        }

        public Dictionary<string, Type> GetPlugins()
        {
            return _plugins;
        }

        public Type GetPlugin(string key)
        {
            if(_plugins.ContainsKey(key))
                return _plugins[key];

            return null;
        }

        public void ClearPlugins()
        {
            _plugins.Clear();
            _pluginTiles.Clear();
        }

        public void AddPluginTiles(string pluginKey, IEnumerable<Type> pluginTiles)
        {
            var plugin = GetPlugin(pluginKey);

            foreach (Type pluginTile in pluginTiles)
            {
                _pluginTiles[plugin].Add(pluginTile);
            }
        }

        public List<Type> GetPluginTiles(string pluginKey)
        {
            return _pluginTiles[_plugins[pluginKey]];
        }
    }
}
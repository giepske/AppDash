using AppDash.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppDash.Server.Plugins
{
    public class PluginResolver
    {
        // <PluginKey>, <AppDashPlugin Type>
        private Dictionary<string, Type> _plugins;
        // <AppDashPlugin Type>, <Tile Types>
        private Dictionary<Type, List<Type>> _pluginTiles;
        // <AppDashPlugin Type>, <Settings Type>
        private Dictionary<Type, Type> _pluginSettings;

        public PluginResolver()
        {
            _plugins = new Dictionary<string, Type>();
            _pluginTiles = new Dictionary<Type, List<Type>>();
            _pluginSettings = new Dictionary<Type, Type>();
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

        public Type GetPlugin(string pluginKey)
        {
            if(_plugins.ContainsKey(pluginKey))
                return _plugins[pluginKey];

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

        public void AddPluginSettings(string pluginKey, Type pluginSettingsType)
        {
            var plugin = GetPlugin(pluginKey);

            _pluginSettings[plugin] = pluginSettingsType;
        }

        public Type GetPluginSettings(string pluginKey)
        {
            if (!_plugins.ContainsKey(pluginKey))
                return null;

            var plugin = _plugins[pluginKey];
            
            if(_pluginSettings.ContainsKey(plugin))
                return _pluginSettings[plugin];

            return null;
        }

        public void SetPluginKey(string oldPluginKey, string newPluginKey)
        {
            var plugin = _plugins[oldPluginKey];

            _plugins[newPluginKey] = plugin;

            _plugins.Remove(oldPluginKey);
        }

        public string GetPluginKey(AppDashPlugin pluginInstance)
        {
            var type = pluginInstance.GetType();

            return _plugins.FirstOrDefault(plugin => plugin.Value == type).Key;
        }
    }
}
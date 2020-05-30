using System;
using System.Collections.Generic;
using AppDash.Plugins.Tiles;

namespace AppDash.Client.Plugins
{
    public class TileResolver
    {
        private Dictionary<string, PluginTileComponent> _tiles;

        public TileResolver()
        {
            _tiles = new Dictionary<string, PluginTileComponent>();
        }

        public PluginTileComponent AddTile(Type tileType)
        {
            var tileInstance = (PluginTileComponent) Activator.CreateInstance(tileType);

            _tiles[tileType.FullName] = tileInstance;

            return tileInstance;
        }

        public Dictionary<string, PluginTileComponent> GetTiles()
        {
            return _tiles;
        }

        public PluginTileComponent GetTile(string key)
        {
            return _tiles[key];
        }

        public void ClearTiles()
        {
            _tiles.Clear();
        }

        public void SetTile(PluginTileComponent component)
        {
            if (_tiles.ContainsKey(component.GetType().FullName))
            {
                var oldComponent = _tiles[component.GetType().FullName];

                component.PluginKey = oldComponent.PluginKey;
                component.PluginSettings = oldComponent.PluginSettings;
                component.Data = oldComponent.Data;

                Console.WriteLine("PluginKey: " + component.PluginKey);
                Console.WriteLine("PluginSettings: " + component.PluginSettings);
                Console.WriteLine("Data: " + component.Data);
            }

            _tiles[component.GetType().FullName] = component;
        }
    }
}
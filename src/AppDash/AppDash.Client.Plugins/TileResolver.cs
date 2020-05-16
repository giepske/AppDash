using System;
using System.Collections.Generic;
using AppDash.Plugins.Tiles;

namespace AppDash.Client.Plugins
{
    public class TileResolver
    {
        private Dictionary<string, TileComponent> _tiles;

        public TileResolver()
        {
            _tiles = new Dictionary<string, TileComponent>();
        }

        public TileComponent AddTile(Type tileType)
        {
            var tileInstance = (TileComponent) Activator.CreateInstance(tileType);

            _tiles.Add(tileType.FullName, tileInstance);

            return tileInstance;
        }

        public Dictionary<string, TileComponent> GetTiles()
        {
            return _tiles;
        }

        public TileComponent GetTile(string key)
        {
            return _tiles[key];
        }

        public void ClearTiles()
        {
            _tiles.Clear();
        }

        public void SetTile(TileComponent component)
        {
            _tiles[component.GetType().FullName] = component;
        }
    }
}
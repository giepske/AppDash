using System;
using System.Collections.Generic;
using System.Linq;
using AppDash.Plugins.Tiles;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AppDash.Client.Plugins
{
    public class TileManager
    {
        private readonly TileResolver _tileResolver;

        public readonly SemaphoreSlim TileLock;

        public TileManager(TileResolver tileResolver)
        {
            _tileResolver = tileResolver;
            TileLock = new SemaphoreSlim(1, 1);
        }
        
        public IEnumerable<TileComponent> LoadTiles(Assembly assembly)
        {
            var tileTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(TileComponent)).ToList();

            foreach (Type tileType in tileTypes)
            {
                yield return _tileResolver.AddTile(tileType);
            }
        }

        public void InitializeTiles()
        {
            //todo initialize tile data
        }

        public async Task<IEnumerable<TileComponent>> GetTiles()
        {
            await TileLock.WaitAsync();

            var tiles = _tileResolver.GetTiles().Values;

            TileLock.Release();

            return tiles;
        }

        public async Task<TileComponent> GetTile(string tileKey)
        {
            await TileLock.WaitAsync();

            var tile = _tileResolver.GetTile(tileKey);

            TileLock.Release();

            return tile;
        }

        public async Task SetTile(TileComponent component)
        {
            await TileLock.WaitAsync();

            _tileResolver.SetTile(component);

            TileLock.Release();
        }
    }
}

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
        private readonly PluginManager _pluginManager;
        private readonly PluginSettingsManager _pluginSettingsManager;

        public readonly SemaphoreSlim TileLock;

        public TileManager(TileResolver tileResolver, PluginManager pluginManager, PluginSettingsManager pluginSettingsManager)
        {
            _tileResolver = tileResolver;
            _pluginManager = pluginManager;
            _pluginSettingsManager = pluginSettingsManager;
            TileLock = new SemaphoreSlim(1, 1);
        }
        
        public IEnumerable<PluginTileComponent> LoadTiles(Assembly assembly)
        {
            var tileTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(PluginTileComponent)).ToList();
            
            foreach (Type tileType in tileTypes)
            {
                yield return _tileResolver.AddTile(tileType);
            }
        }

        public async Task InitializeTiles()
        {
            foreach (var pluginTileComponent in _tileResolver.GetTiles().Values)
            {
                var plugin = _pluginManager.GetPlugin(pluginTileComponent);

                pluginTileComponent.PluginKey = plugin.Key;
                pluginTileComponent.PluginSettings = await _pluginSettingsManager.GetPluginSettings(plugin.Key);
            }
        }

        public async Task<IEnumerable<PluginTileComponent>> GetTiles()
        {
            await TileLock.WaitAsync();

            var tiles = _tileResolver.GetTiles().Values;

            TileLock.Release();

            return tiles;
        }

        public async Task<PluginTileComponent> GetTile(string tileKey)
        {
            await TileLock.WaitAsync();

            var tile = _tileResolver.GetTile(tileKey);

            TileLock.Release();

            return tile;
        }

        public async Task SetTile(PluginTileComponent component)
        {
            await TileLock.WaitAsync();

            _tileResolver.SetTile(component);

            TileLock.Release();
        }
    }
}

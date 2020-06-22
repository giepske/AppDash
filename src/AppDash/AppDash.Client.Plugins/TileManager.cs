using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppDash.Plugins.Tiles;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Plugins;
using Newtonsoft.Json;

namespace AppDash.Client.Plugins
{
    public class TileManager
    {
        private readonly PluginResolver _pluginResolver;
        private readonly PluginManager _pluginManager;
        private readonly PluginSettingsManager _pluginSettingsManager;

        public readonly SemaphoreSlim TileLock;

        public TileManager(PluginResolver pluginResolver, PluginManager pluginManager, PluginSettingsManager pluginSettingsManager)
        {
            _pluginResolver = pluginResolver;
            _pluginManager = pluginManager;
            _pluginSettingsManager = pluginSettingsManager;
            TileLock = new SemaphoreSlim(1, 1);
        }
        
        public IEnumerable<PluginTileComponent> LoadTiles(Assembly assembly, List<Tuple<string, Type>> tileKeys)
        {
            var tileTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(PluginTileComponent)).ToList();
            
            foreach (Type tileType in tileTypes)
            {
                var tileKeys2 = tileKeys.Where(e => e.Item2 == tileType).ToList();

                foreach (Tuple<string, Type> tileKey in tileKeys2)
                {
                    tileKeys.Remove(tileKey);

                    Console.WriteLine($"Loaded tile: {tileKey.Item2.Name} {tileKey.Item1}");

                    yield return _pluginResolver.LoadPluginTileComponent(tileType, tileKey.Item1);
                }
            }
        }

        public async Task InitializeTiles()
        {
            foreach (var pluginTileComponent in _pluginResolver.GetPluginTileComponents())
            {
                var plugin = _pluginManager.GetPluginInstance(pluginTileComponent);

                pluginTileComponent.PluginKey = plugin.Key;
                pluginTileComponent.PluginSettings = await _pluginSettingsManager.GetPluginSettings(plugin.Key);
            }
        }

        public async Task<IEnumerable<PluginTileComponent>> GetPluginTileComponents()
        {
            await TileLock.WaitAsync();

            var tiles = _pluginResolver.GetPluginTileComponents();

            TileLock.Release();

            return tiles;
        }

        public async Task<PluginTileComponent> GetPluginTileComponent(string pluginKey, string tileKey)
        {
            await TileLock.WaitAsync();

            var tile = _pluginResolver.GetPluginTileComponent(pluginKey, tileKey);

            TileLock.Release();

            return tile;
        }

        public async Task SetPluginTileComponent(string tileKey, PluginTileComponent component)
        {
            await TileLock.WaitAsync();

            _pluginResolver.SetPluginTileComponent(tileKey, component);

            TileLock.Release();
        }
    }
}

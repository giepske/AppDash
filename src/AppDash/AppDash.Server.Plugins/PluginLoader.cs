using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using AppDash.Server.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Plugins
{
    public class PluginLoader
    {
        private readonly PluginResolver _pluginResolver;
        private readonly IServiceProvider _serviceProvider;
        
        public PluginLoader(PluginResolver pluginResolver, IServiceProvider serviceProvider)
        {
            _pluginResolver = pluginResolver;
            _serviceProvider = serviceProvider;
        }

        public void LoadPlugins(string pluginPath)
        {
            _pluginResolver.ClearPlugins();

            List<string> plugins = Directory.GetDirectories(pluginPath)
                .Where(directory => File.Exists(Path.Combine(directory, Path.GetFileName(directory) + ".dll")))
                .Select(directory => Path.Combine(directory, Path.GetFileName(directory) + ".dll")).ToList();

            foreach (string plugin in plugins)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(plugin);

                    LoadAppDashPlugins(assembly);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            Console.WriteLine($"Loaded {_pluginResolver.GetPluginInstances().Count()} plugin(s).");
        }

        private void LoadAppDashPlugins(Assembly assembly)
        {
            var pluginTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(AppDashPlugin)).ToList();

            foreach (Type pluginType in pluginTypes)
            {
                string pluginKey = GetPluginKey(pluginType) ?? InsertPluginKey(pluginType);

                var plugin = _pluginResolver.AddPlugin(pluginType, pluginKey);

                LoadPluginPages(pluginType);
                LoadPluginTiles(pluginType, plugin.PluginInstance);
                LoadPluginSettings(pluginType);
                LoadPluginControllers(pluginType);
            }
        }
        
        private void LoadPluginPages(Type pluginType)
        {
            _pluginResolver.LoadPluginPages(pluginType);
        }

        private void LoadPluginSettings(Type pluginType)
        {
            _pluginResolver.LoadPluginSettings(pluginType);
        }

        private void LoadPluginTiles(Type pluginType, AppDashPlugin pluginInstance)
        {
            var pluginTileTypes = pluginType.Assembly.GetTypes().Where(type =>
                (type.BaseType?.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.BaseType?.GetGenericTypeDefinition() == typeof(PluginTile<,>) &&
                type.BaseType?.GenericTypeArguments[0] == pluginType &&
                type.GetCustomAttribute<FactoryTileAttribute>() == null);

            foreach (Type tileType in pluginTileTypes)
            {
                string tileKey = GetTileKey(tileType) ?? InsertTileKey(tileType);
                
                _pluginResolver.LoadPluginTile(pluginType, tileType, tileKey);
            }

            var tileFactoryTypes = pluginType.Assembly.GetTypes().Where(type => 
                type.GetInterfaces().FirstOrDefault(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ITileFactory<>))
                ?.GenericTypeArguments[0] == pluginType);

            foreach (Type tileFactoryType in tileFactoryTypes)
            {
                var tileFactory = Activator.CreateInstance(tileFactoryType);

                var method = tileFactoryType.GetMethod("GetTiles");

                var tiles = ((IEnumerable<ITile>)method.Invoke(tileFactory, new[] {pluginInstance})).ToList();

                for (int i = 0; i < tiles.Count; i++)
                {
                    var tileType = tiles[i].GetType();

                    string tileKey = GetTileKey(tileType, tileFactoryType.FullName, i) ?? InsertTileKey(tileType, tileFactoryType.FullName, i);

                    _pluginResolver.LoadPluginTile(pluginType, tiles[i], tileKey);
                }
            }
        }

        private void LoadPluginControllers(Type pluginType)
        {
            _pluginResolver.LoadPluginControllers(pluginType);
        }

        private string InsertPluginKey(Type pluginType)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var pluginRepository = scope.ServiceProvider.GetService<IRepository<Core.Domain.Plugins.Plugin>>();

                AppDashPlugin pluginInstance = (AppDashPlugin)Activator.CreateInstance(pluginType);

                var pluginData = new Core.Domain.Plugins.Plugin
                {
                    UniqueIdentifier = pluginType.FullName,
                    Key = Guid.NewGuid().ToString("N"),
                    Name = pluginInstance.Name
                };

                pluginRepository.Insert(pluginData);

                return pluginData.Key;
            }
        }

        private string GetPluginKey(Type pluginType)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var pluginRepository = scope.ServiceProvider.GetService<IRepository<Core.Domain.Plugins.Plugin>>();

                return pluginRepository.TableNoTracking.FirstOrDefault(e => e.UniqueIdentifier == pluginType.FullName)?.Key;
            }
        }

        private string InsertTileKey(Type tileType)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tileRepository = scope.ServiceProvider.GetService<IRepository<Core.Domain.Tiles.Tile>>();

                var tileData = new Core.Domain.Tiles.Tile
                {
                    UniqueIdentifier = tileType.FullName,
                    Key = Guid.NewGuid().ToString("N")
                };

                tileRepository.Insert(tileData);

                return tileData.Key;
            }
        }

        private string GetTileKey(Type tileType)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tileRepository = scope.ServiceProvider.GetService<IRepository<Core.Domain.Tiles.Tile>>();

                return tileRepository.TableNoTracking.FirstOrDefault(e => e.UniqueIdentifier == tileType.FullName)?.Key;
            }
        }

        private string InsertTileKey(Type tileType, string tileFactory, int factoryIndex)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tileRepository = scope.ServiceProvider.GetService<IRepository<Core.Domain.Tiles.Tile>>();

                var tileData = new Core.Domain.Tiles.Tile
                {
                    UniqueIdentifier = tileType.FullName,
                    Key = Guid.NewGuid().ToString("N"),
                    TileFactory = tileFactory,
                    FactoryIndex = factoryIndex
                };

                tileRepository.Insert(tileData);

                return tileData.Key;
            }
        }

        private string GetTileKey(Type tileType, string tileFactory, int factoryIndex)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tileRepository = scope.ServiceProvider.GetService<IRepository<Core.Domain.Tiles.Tile>>();

                return tileRepository.TableNoTracking.FirstOrDefault(e => 
                    e.UniqueIdentifier == tileType.FullName && e.TileFactory == tileFactory && e.FactoryIndex == factoryIndex)?.Key;
            }
        }
    }
}
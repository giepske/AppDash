using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace AppDash.Server.Plugins
{
    public class PluginManager
    {
        private readonly PluginResolver _pluginResolver;

        private readonly IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;

        public PluginManager(PluginResolver pluginResolver, IServiceProvider serviceProvider)
        {
            _pluginResolver = pluginResolver;

            _serviceCollection = new ServiceCollection()
                .AddDependencies(serviceProvider);
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

            _serviceProvider = _serviceCollection.BuildServiceProvider();

            Console.WriteLine($"Loaded {_pluginResolver.GetPlugins().Count} plugin(s).");
        }

        private void LoadAppDashPlugins(Assembly assembly)
        {
            var pluginTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(AppDashPlugin)).ToList();

            foreach (Type pluginType in pluginTypes)
            {
                string pluginKey = _pluginResolver.AddPlugin(pluginType);

                _serviceCollection.AddSingleton(pluginType);
                _serviceCollection.AddSingleton(typeof(AppDashPlugin), serviceProvider => serviceProvider.GetService(pluginType));

                LoadPluginTiles(pluginKey, pluginType, assembly);
            }
        }

        private void LoadPluginTiles(string pluginKey, Type pluginType, Assembly assembly)
        {
            var pluginTileTypes = assembly.GetTypes().Where(type => 
                (type.BaseType?.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.BaseType.GetGenericTypeDefinition() == typeof(Tile<,>) &&
                type.BaseType?.GenericTypeArguments.FirstOrDefault() == pluginType).ToList();

            foreach (Type pluginTileType in pluginTileTypes)
            {
                _serviceCollection.AddSingleton(typeof(ITile), pluginTileType);
            }

            _pluginResolver.AddPluginTiles(pluginKey, pluginTileTypes);
        }

        public T GetPlugin<T>(string key)
        {
            Type pluginType = _pluginResolver.GetPlugin(key);

            return (T)_serviceProvider.GetService(pluginType);
        }

        public IEnumerable<AppDashPlugin> GetPlugins()
        {
            return _serviceProvider.GetServices<AppDashPlugin>();
        }

        public void InitializePlugins()
        {
            List<AppDashPlugin> pluginInstances = _serviceProvider.GetServices<AppDashPlugin>().ToList();

            foreach (KeyValuePair<string, Type> plugin in _pluginResolver.GetPlugins())
            {
                AppDashPlugin pluginInstance =
                    pluginInstances.First(pluginInstance1 => pluginInstance1.GetType() == plugin.Value);

                pluginInstance.Key = plugin.Key;

                InitializePluginTiles(pluginInstance.Key);
            }
        }

        public void InitializePluginTiles(string pluginKey)
        {
            var pluginTileInstances = _serviceProvider.GetServices<ITile>().ToList();

            var pluginTiles = _pluginResolver.GetPluginTiles(pluginKey);

            foreach (Type pluginTile in pluginTiles)
            {
                ITile pluginTileInstance =
                    pluginTileInstances.First(pluginTileInstance1 => pluginTileInstance1.GetType() == pluginTile);

                var setDependenciesMethod = pluginTileInstance.GetType().GetMethod("SetDependencies");

                if (setDependenciesMethod != null)
                {
                    object?[]? dependencies = setDependenciesMethod.GetParameters()
                        .Select(parameter =>
                        {
                            return _serviceProvider.GetService(parameter.ParameterType);
                        })
                        .ToArray();

                    setDependenciesMethod.Invoke(pluginTileInstance, dependencies);
                }

                pluginTileInstance.OnAfterLoad().Wait();
            }
        }

        public IEnumerable<ITile> GetTiles()
        {
            return _serviceProvider.GetService<List<ITile>>();
        }
    }
}

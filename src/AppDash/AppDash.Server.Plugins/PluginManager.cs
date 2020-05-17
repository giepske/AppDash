using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AppDash.Plugins;
using AppDash.Plugins.Settings;
using AppDash.Plugins.Tiles;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
                LoadPluginSettings(pluginKey, pluginType, assembly);
            }
        }

        private void LoadPluginSettings(string pluginKey, Type pluginType, Assembly assembly)
        {
            var pluginSettingsType = assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType.GetGenericTypeDefinition() == typeof(Settings<,>) &&
                type.BaseType?.GenericTypeArguments.FirstOrDefault() == pluginType);

            //this plugin doesn't have a settings page.
            if (pluginSettingsType == null)
                return;

            _serviceCollection.AddSingleton(typeof(ISettings), pluginSettingsType);

            _pluginResolver.AddPluginSettings(pluginKey, pluginSettingsType);
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
            var pluginRepository = _serviceProvider.GetService<IRepository<Plugin>>();

            List<AppDashPlugin> pluginInstances = _serviceProvider.GetServices<AppDashPlugin>().ToList();

            foreach (KeyValuePair<string, Type> plugin in _pluginResolver.GetPlugins().ToList())
            {
                AppDashPlugin pluginInstance =
                    pluginInstances.First(pluginInstance1 => pluginInstance1.GetType() == plugin.Value);

                var pluginData =
                    pluginRepository.TableNoTracking.FirstOrDefault(e => e.UniqueIdentifier == plugin.Value.FullName);

                if (pluginData == null)
                {
                    pluginData = new Plugin
                    {
                        UniqueIdentifier = pluginInstance.GetType().FullName,
                        Key = _pluginResolver.GetPluginKey(pluginInstance),
                        Name = pluginInstance.Name
                    };

                    pluginRepository.Insert(pluginData);
                }
                else
                {
                    _pluginResolver.SetPluginKey(plugin.Key, pluginData.Key);
                }

                pluginInstance.Key = pluginData.Key;

                InitializePluginTiles(pluginInstance.Key);
                InitializePluginSettings(pluginInstance.Key);
            }
        }

        private void InitializePluginSettings(string pluginKey)
        {
            var pluginSettingsInstances = _serviceProvider.GetServices<ISettings>().ToList();

            var pluginSettings = _pluginResolver.GetPluginSettings(pluginKey);

            //this plugin doesn't have a settings page.
            if (pluginSettings == null)
                return;

            ISettings pluginSettingsInstance =
                pluginSettingsInstances.First(pluginTileInstance1 => pluginTileInstance1.GetType() == pluginSettings);

            var initializeMethod = pluginSettingsInstance.GetType().BaseType?.GetMethod("Initialize", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (initializeMethod != null)
            {
                object?[]? dependencies = initializeMethod.GetParameters()
                    .Select(parameter =>
                    {
                        return _serviceProvider.GetService(parameter.ParameterType);
                    })
                    .ToArray();

                initializeMethod.Invoke(pluginSettingsInstance, dependencies);
            }

            pluginSettingsInstance.OnAfterLoad().Wait();
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

        public PluginData GetPluginSettings(string pluginKey)
        {
            var settingsType = _pluginResolver.GetPluginSettings(pluginKey);

            if (settingsType == null)
                return null;

            var settingsList = _serviceProvider.GetServices<ISettings>().ToList();

            var settings = settingsList.FirstOrDefault(settings => settings.GetType() == settingsType);

            return settings?.SettingsData;
        }

        public void SetPluginSettings(string pluginKey, PluginData pluginData)
        {
            var settingsType = _pluginResolver.GetPluginSettings(pluginKey);

            var settingsList = _serviceProvider.GetServices<ISettings>().ToList();

            var settings = settingsList.FirstOrDefault(settings => settings.GetType() == settingsType);

            settings.SettingsData.Data = pluginData.Data;

            var pluginSettingsRepository = _serviceProvider.GetService<IRepository<PluginSettings>>();

            var pluginSettings = pluginSettingsRepository.Table.FirstOrDefault(pluginSettings => pluginSettings.PluginKey == pluginKey);

            pluginSettings.Data = System.Text.Json.JsonSerializer.Serialize(pluginData, new JsonSerializerOptions
            {
                Converters = { new PluginDataConverter() }
            });

            pluginSettingsRepository.Update(pluginSettings);
        }
    }
}

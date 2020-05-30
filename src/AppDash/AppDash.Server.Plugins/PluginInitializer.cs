using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AppDash.Plugins;
using AppDash.Plugins.Settings;
using AppDash.Plugins.Tiles;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Plugins
{
    public class PluginInitializer
    {
        private readonly PluginResolver _pluginResolver;
        private readonly PluginSettingsManager _pluginSettingsManager;

        public PluginInitializer(PluginResolver pluginResolver, PluginSettingsManager pluginSettingsManager)
        {
            _pluginResolver = pluginResolver;
            _pluginSettingsManager = pluginSettingsManager;
        }

        public void InitializePlugins(IServiceProvider serviceProvider)
        {
            var pluginRepository = serviceProvider.GetService<IRepository<Plugin>>();

            List<AppDashPlugin> pluginInstances = serviceProvider.GetServices<AppDashPlugin>().ToList();

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
                
                _pluginResolver.SetServiceProvider(pluginInstance.Key, pluginInstance);

                InitializePluginTiles(pluginInstance.Key, serviceProvider);
                InitializePluginSettings(pluginInstance.Key, serviceProvider);
            }
        }

        private void InitializePluginSettings(string pluginKey, IServiceProvider serviceProvider)
        {
            var pluginSettingsInstances = serviceProvider.GetServices<ISettings>().ToList();

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
                        return serviceProvider.GetService(parameter.ParameterType);
                    })
                    .ToArray();

                initializeMethod.Invoke(pluginSettingsInstance, dependencies);
            }

            pluginSettingsInstance.OnAfterLoad().Wait();
        }

        public void InitializePluginTiles(string pluginKey, IServiceProvider serviceProvider)
        {
            var pluginTileInstances = serviceProvider.GetServices<ITile>().ToList();

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
                            return serviceProvider.GetService(parameter.ParameterType);
                        })
                        .ToArray();

                    setDependenciesMethod.Invoke(pluginTileInstance, dependencies);
                }

                var pluginSettingsProperty = pluginTileInstance.GetType().GetProperty("PluginSettings");

                pluginSettingsProperty?.SetValue(pluginTileInstance, _pluginSettingsManager.GetPluginSettings(pluginKey) ?? new PluginData());

                pluginTileInstance.OnAfterLoad().Wait();
            }
        }
    }
}
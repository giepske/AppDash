using System;
using System.Linq;
using System.Reflection;
using AppDash.Plugins;
using AppDash.Plugins.Pages;
using AppDash.Plugins.Tiles;

namespace AppDash.Server.Plugins
{
    public class PluginInitializer
    {
        private readonly PluginResolver _pluginResolver;
        private readonly PluginSettingsManager _pluginSettingsManager;
        private readonly IServiceProvider _serviceProvider;

        public PluginInitializer(PluginResolver pluginResolver, PluginSettingsManager pluginSettingsManager, IServiceProvider serviceProvider)
        {
            _pluginResolver = pluginResolver;
            _pluginSettingsManager = pluginSettingsManager;
            _serviceProvider = serviceProvider;
        }

        public void InitializePlugins()
        {
            foreach (AppDashPlugin plugin in _pluginResolver.GetPluginInstances())
            {
                InitializePluginTiles(plugin.Key);
                InitializePluginSettings(plugin.Key);
                InitializePluginPages(plugin.Key);
            }
        }

        private void InitializePluginSettings(string pluginKey)
        {
            var pluginSettingsInstance = _pluginResolver.GetSettings(pluginKey);

            //this plugin doesn't have a settings page.
            if (pluginSettingsInstance == null)
                return;

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

        private void InitializePluginTiles(string pluginKey)
        {
            var pluginTileInstances = _pluginResolver.GetPluginTiles(pluginKey);

            foreach (ITile pluginTileInstance in pluginTileInstances)
            {
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

                var pluginSettingsProperty = pluginTileInstance.GetType().GetProperty("PluginSettings");

                pluginSettingsProperty?.SetValue(pluginTileInstance, _pluginSettingsManager.GetPluginSettings(pluginKey));

                pluginTileInstance.OnAfterLoad().Wait();

                pluginTileInstance.OnAfterLoadInternal().Wait();
            }
        }

        private void InitializePluginPages(string pluginKey)
        {
            var pluginPageInstances = _pluginResolver.GetPluginPages(pluginKey);

            foreach (IPage pluginTileInstance in pluginPageInstances)
            {
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

                var pluginSettingsProperty = pluginTileInstance.GetType().GetProperty("PluginSettings");

                pluginSettingsProperty?.SetValue(pluginTileInstance, _pluginSettingsManager.GetPluginSettings(pluginKey));

                pluginTileInstance.OnAfterLoad().Wait();
            }
        }
    }
}
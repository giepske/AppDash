using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Settings;
using AppDash.Plugins.Tiles;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace AppDash.Client.Plugins
{
    public class PluginManager
    {
        private readonly PluginResolver _pluginResolver;

        public readonly SemaphoreSlim PluginLock;

        public PluginManager(PluginResolver pluginResolver)
        {
            _pluginResolver = pluginResolver;
            PluginLock = new SemaphoreSlim(1, 1);
        }
        
        public async Task<IEnumerable<AppDashPlugin>> GetPluginInstances()
        {
            await PluginLock.WaitAsync();

            var plugins = _pluginResolver.GetPluginInstances();

            PluginLock.Release();

            return plugins;
        }

        public void LoadPlugins(Assembly assembly, Dictionary<string, string> pluginKeys)
        {
            var pluginTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(AppDashPlugin)).ToList();

            foreach (Type pluginType in pluginTypes)
            {
                var pluginKey = pluginKeys.FirstOrDefault(e => e.Value == pluginType.Name).Key;

                var plugin = _pluginResolver.AddPlugin(pluginType, pluginKey);

                Console.WriteLine($"{plugin.PluginInstance.Name} set key to {pluginKey}");
            }
        }

        public AppDashPlugin GetPluginInstance(PluginTileComponent pluginTileComponent)
        {
            var tileTypes = pluginTileComponent.GetType().Assembly.GetTypes()
                .Where(type => typeof(ITile).IsAssignableFrom(type)).ToList();

            if (!tileTypes.Any())
                return null;

            var pluginType = tileTypes.FirstOrDefault(tileType => tileType.BaseType?.GenericTypeArguments[1] == pluginTileComponent.GetType())
                ?.BaseType?.GenericTypeArguments.FirstOrDefault();

            if (pluginType == null)
                return null;

            return _pluginResolver.GetPluginInstance(pluginType);
        }

        public AppDashPlugin GetPluginInstance(PluginSettingsComponent pluginSettingsComponent)
        {
            var settingsTypes = pluginSettingsComponent.GetType().Assembly.GetTypes()
                .Where(type => typeof(ISettings).IsAssignableFrom(type)).ToList();

            if (!settingsTypes.Any())
                return null;

            var pluginType = settingsTypes.FirstOrDefault(settingsType => settingsType.BaseType?.GenericTypeArguments[1] == pluginSettingsComponent.GetType())
                ?.BaseType?.GenericTypeArguments.FirstOrDefault();

            if (pluginType == null)
                return null;

            return _pluginResolver.GetPluginInstance(pluginType);
        }
    }
}
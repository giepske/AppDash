using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace AppDash.Client.Plugins
{
    public class PluginManager
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _navigationManager;
        private readonly PluginResolver _pluginResolver;

        public readonly SemaphoreSlim PluginLock;

        private List<Assembly> _assemblies;

        public PluginManager(HttpClient httpClient, NavigationManager navigationManager, PluginResolver pluginResolver)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _pluginResolver = pluginResolver;
            _assemblies = new List<Assembly>();
            PluginLock = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Download a plugin assembly from the url and load it as an assembly.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Assembly> LoadAssembly(string url)
        {
            var assemblyBytes = await _httpClient.GetByteArrayAsync(url);

            var assembly = Assembly.Load(assemblyBytes);

            _assemblies.Add(assembly);

            return assembly;
        }

        public async Task<IEnumerable<Assembly>> GetAssemblies()
        {
            await PluginLock.WaitAsync();

            var assemblies = _assemblies;

            PluginLock.Release();

            return assemblies;
        }

        public async Task<IEnumerable<AppDashPlugin>> GetPlugins()
        {
            await PluginLock.WaitAsync();

            var plugins = _pluginResolver.GetPlugins().Values;

            PluginLock.Release();

            return plugins;
        }

        public void LoadPlugins(Assembly assembly, Dictionary<string, string> pluginKeys)
        {
            var pluginTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(AppDashPlugin)).ToList();

            foreach (Type pluginType in pluginTypes)
            {
                var plugin = _pluginResolver.AddPlugin(pluginType);

                var pluginKey = pluginKeys.FirstOrDefault(e => e.Value == plugin.GetType().Name).Key;

                plugin.Key = pluginKey;

                Console.WriteLine($"{plugin.Name} set key to {pluginKey}");
            }
        }

        public AppDashPlugin GetPlugin(PluginTileComponent pluginTileComponent)
        {
            var tileTypes = pluginTileComponent.GetType().Assembly.GetTypes()
                .Where(type => typeof(ITile).IsAssignableFrom(type)).ToList();

            if (!tileTypes.Any())
                return null;

            var pluginType = tileTypes.FirstOrDefault(tileType => tileType.BaseType?.GenericTypeArguments[1] == pluginTileComponent.GetType())
                ?.BaseType?.GenericTypeArguments.FirstOrDefault();

            if (pluginType == null)
                return null;

            return _pluginResolver.GetPlugin(pluginType);
        }
    }
}
using AppDash.Plugins.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;

namespace AppDash.Client.Plugins
{
    public class SettingsManager
    {
        private readonly PluginManager _pluginManager;
        private readonly HttpClient _httpClient;
        private readonly PluginResolver _pluginResolver;
        private readonly PluginSettingsManager _pluginSettingsManager;

        public readonly SemaphoreSlim SettingsLock;
        public readonly SemaphoreSlim TileLock;

        public SettingsManager(PluginManager pluginManager, HttpClient httpClient, PluginResolver pluginResolver, PluginSettingsManager pluginSettingsManager)
        {
            _pluginManager = pluginManager;
            _httpClient = httpClient;
            _pluginResolver = pluginResolver;
            _pluginSettingsManager = pluginSettingsManager;
            SettingsLock = new SemaphoreSlim(1, 1);
            TileLock = new SemaphoreSlim(1, 1);
        }
        
        public IEnumerable<PluginSettingsComponent> LoadSettings(Assembly assembly)
        {
            var settingsTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(PluginSettingsComponent)).ToList();

            foreach (Type settingsType in settingsTypes)
            {
                Console.WriteLine("LoadSettings " + settingsType);

                var settings = _pluginResolver.LoadPluginSettingsComponent(settingsType, _httpClient);

                if(settings != null)
                    yield return settings;
            }
        }

        public async Task InitializeSettings()
        {
            foreach (var pluginSettingsComponent in _pluginResolver.GetSettingComponents())
            {
                var plugin = _pluginManager.GetPluginInstance(pluginSettingsComponent);

                pluginSettingsComponent.PluginKey = plugin.Key;
                pluginSettingsComponent.SettingsData = await _pluginSettingsManager.GetPluginSettings(plugin.Key);
            }
        }

        public async Task<IEnumerable<PluginSettingsComponent>> GetSettingComponents()
        {
            await SettingsLock.WaitAsync();

            var settings = _pluginResolver.GetSettingComponents();

            SettingsLock.Release();

            return settings;
        }

        public async Task<PluginSettingsComponent> GetSettingComponent(string pluginKey)
        {
            await SettingsLock.WaitAsync();

            var settings = _pluginResolver.GetSettingComponent(pluginKey);

            SettingsLock.Release();

            return settings;
        }

        public async Task SetPluginSettingsComponent(PluginSettingsComponent component)
        {
            await SettingsLock.WaitAsync();

            _pluginResolver.SetPluginSettingsComponent(component);

            SettingsLock.Release();
        }

        //public async Task SetPluginTileComponent(PluginTileComponent component)
        //{
        //    await TileLock.WaitAsync();

        //    _pluginResolver.SetPluginTileComponent(component);

        //    TileLock.Release();
        //}
    }
}

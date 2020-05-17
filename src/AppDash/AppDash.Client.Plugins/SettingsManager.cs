using AppDash.Plugins.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Plugins;

namespace AppDash.Client.Plugins
{
    public class SettingsManager
    {
        private readonly SettingsResolver _settingsResolver;
        private readonly HttpClient _httpClient;

        public readonly SemaphoreSlim SettingsLock;

        public SettingsManager(SettingsResolver settingsResolver, HttpClient httpClient)
        {
            _settingsResolver = settingsResolver;
            _httpClient = httpClient;
            SettingsLock = new SemaphoreSlim(1, 1);
        }
        
        public IEnumerable<SettingsComponent> LoadSettings(Assembly assembly)
        {
            var settingsTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(SettingsComponent)).ToList();

            foreach (Type settingsType in settingsTypes)
            {
                yield return _settingsResolver.AddSettings(settingsType, _httpClient);
            }
        }

        public void InitializeSettings()
        {
            //todo initialize settings data
        }

        public async Task<Dictionary<string, SettingsComponent>> GetSettings()
        {
            await SettingsLock.WaitAsync();

            var settings = _settingsResolver.GetSettings();

            SettingsLock.Release();

            return settings;
        }

        public async Task<SettingsComponent> GetSettings(string settingsKey)
        {
            await SettingsLock.WaitAsync();

            var settings = _settingsResolver.GetSettings(settingsKey);

            SettingsLock.Release();

            return settings;
        }

        public async Task SetSettings(string pluginKey, SettingsComponent component)
        {
            await SettingsLock.WaitAsync();

            await _settingsResolver.SetSettings(pluginKey, component);

            SettingsLock.Release();
        }

        public async Task<PluginData> SetSettingsData(string pluginKey)
        {
            return await _settingsResolver.SetSettingsData(pluginKey);
        }
    }
}

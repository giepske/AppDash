using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Plugins;
using AppDash.Plugins.Pages;
using AppDash.Plugins.Settings;

namespace AppDash.Client.Plugins
{
    public class SettingsResolver
    {
        private readonly PluginResolver _pluginResolver;
        private readonly HttpClient _httpClient;

        private Dictionary<string, SettingsComponent> _settings;
        private Dictionary<string, PluginData> _settingsData;

        public SettingsResolver(PluginResolver pluginResolver, HttpClient httpClient)
        {
            _pluginResolver = pluginResolver;
            _httpClient = httpClient;
            _settings = new Dictionary<string, SettingsComponent>();
            _settingsData = new Dictionary<string, PluginData>();
        }

        public SettingsComponent AddSettings(Type settingsType, HttpClient httpClient)
        {
            var settingsInstance = (SettingsComponent) Activator.CreateInstance(settingsType);

            var pluginType = settingsType.Assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.GetGenericTypeDefinition() == typeof(Settings<,>) &&
                type.BaseType?.GenericTypeArguments[1] == settingsType)
                .BaseType.GenericTypeArguments[0];

            var pluginInstance = _pluginResolver.GetPlugin(pluginType);

            _settings[pluginInstance.Key] = settingsInstance;

            settingsInstance.HttpClient = httpClient;
            settingsInstance.PluginKey = pluginInstance.Key;

            return settingsInstance;
        }

        public Dictionary<string, SettingsComponent> GetSettings()
        {
            return _settings;
        }

        public SettingsComponent GetSettings(string pluginKey)
        {
            return _settings[pluginKey];
        }

        public void ClearSettings()
        {
            _settings.Clear();
        }

        public async Task SetSettings(string pluginKey, SettingsComponent component)
        {
            if (_settings.ContainsKey(pluginKey))
            {
                var oldComponent = _settings[pluginKey];

                Console.WriteLine("oldComponent.HttpClient " + oldComponent.HttpClient);

                component.PluginKey = oldComponent.PluginKey;
                component.HttpClient = oldComponent.HttpClient;
                component.SettingsData = await SetSettingsData(pluginKey);
            }

            _settings[pluginKey] = component;
        }

        public async Task<PluginData> SetSettingsData(string pluginKey)
        {
            if (_settingsData.ContainsKey(pluginKey))
                return _settingsData[pluginKey];

            var result = await _httpClient.GetJson<ApiResult>($"api/plugins/{pluginKey}/settings");

            var settingsData = result.GetData<PluginData>();

            _settingsData[pluginKey] = settingsData;

            _settings[pluginKey].SettingsData = settingsData;

            return settingsData;
        }
    }
}
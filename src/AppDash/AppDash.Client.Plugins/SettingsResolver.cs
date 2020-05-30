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
        private readonly PluginSettingsManager _pluginSettingsManager;

        private Dictionary<string, PluginSettingsComponent> _settings;

        public SettingsResolver(PluginResolver pluginResolver, HttpClient httpClient, PluginSettingsManager pluginSettingsManager)
        {
            _pluginResolver = pluginResolver;
            _httpClient = httpClient;
            _pluginSettingsManager = pluginSettingsManager;
            _settings = new Dictionary<string, PluginSettingsComponent>();
        }

        public PluginSettingsComponent AddSettings(Type settingsType, HttpClient httpClient)
        {
            var settingsInstance = (PluginSettingsComponent) Activator.CreateInstance(settingsType);

            var pluginType = settingsType.Assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.GetGenericTypeDefinition() == typeof(PluginSettings<,>) &&
                type.BaseType?.GenericTypeArguments[1] == settingsType)
                .BaseType.GenericTypeArguments[0];

            var pluginInstance = _pluginResolver.GetPlugin(pluginType);

            _settings[pluginInstance.Key] = settingsInstance;

            settingsInstance.HttpClient = httpClient;
            settingsInstance.PluginKey = pluginInstance.Key;

            return settingsInstance;
        }

        public Dictionary<string, PluginSettingsComponent> GetSettings()
        {
            return _settings;
        }

        public PluginSettingsComponent GetSettings(string pluginKey)
        {
            return _settings[pluginKey];
        }

        public void ClearSettings()
        {
            _settings.Clear();
        }

        public async Task SetSettings(string pluginKey, PluginSettingsComponent component)
        {
            if (_settings.ContainsKey(pluginKey))
            {
                var oldComponent = _settings[pluginKey];

                component.PluginKey = oldComponent.PluginKey;
                component.HttpClient = oldComponent.HttpClient;
                component.SettingsData = await _pluginSettingsManager.GetPluginSettings(pluginKey);

                await component.Initialize();
            }

            _settings[pluginKey] = component;
        }
    }
}
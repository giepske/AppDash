using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using AppDash.Plugins;
using AppDash.Plugins.Controllers;
using AppDash.Plugins.Settings;
using AppDash.Plugins.Tiles;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Plugins
{
    public class PluginManager
    {
        private readonly PluginResolver _pluginResolver;
        private readonly PluginLoader _pluginLoader;
        private readonly PluginInitializer _pluginInitializer;

        private IServiceProvider _serviceProvider;

        public PluginManager(PluginResolver pluginResolver, PluginLoader pluginLoader, PluginInitializer pluginInitializer)
        {
            _pluginResolver = pluginResolver;
            _pluginLoader = pluginLoader;
            _pluginInitializer = pluginInitializer;
        }

        public void LoadPlugins(string pluginPath)
        {
            _serviceProvider = _pluginLoader.LoadPlugins(pluginPath);

            _pluginInitializer.InitializePlugins(_serviceProvider);
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

        public IEnumerable<ITile> GetTiles()
        {
            return _serviceProvider.GetServices<ITile>();
        }

        public PluginData GetPluginSettings(string pluginKey)
        {
            var settingsType = _pluginResolver.GetPluginSettings(pluginKey);

            if (settingsType == null)
                return null;

            var settingsList = _serviceProvider.GetServices<ISettings>().ToList();

            var settings = settingsList.FirstOrDefault(settings1 => settings1.GetType() == settingsType);

            return settings?.SettingsData;
        }

        public void SetPluginSettings(string pluginKey, PluginData pluginData)
        {
            var settingsType = _pluginResolver.GetPluginSettings(pluginKey);

            var settingsList = _serviceProvider.GetServices<ISettings>().ToList();

            var settings = settingsList.FirstOrDefault(settings => settings.GetType() == settingsType);

            settings.SettingsData.Data = pluginData.Data;

            var pluginSettingsRepository = _serviceProvider.GetService<IRepository<PluginSettings>>();

            var pluginSettings = pluginSettingsRepository.Table.FirstOrDefault(pluginSettings1 => pluginSettings1.PluginKey == pluginKey);

            pluginSettings.Data = JsonSerializer.Serialize(pluginData, new JsonSerializerOptions
            {
                Converters = { new PluginDataConverter() }
            });

            pluginSettingsRepository.Update(pluginSettings);
        }

        public IEnumerable<IPluginController> GetPluginControllers(string pluginKey)
        {
            return _pluginResolver.GetPluginControllers(pluginKey);
        }
    }
}

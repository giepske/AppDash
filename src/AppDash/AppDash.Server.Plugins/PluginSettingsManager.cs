using System;
using System.Collections.Generic;
using System.Linq;
using AppDash.Plugins;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Plugin = AppDash.Server.Core.Domain.Plugins.Plugin;

namespace AppDash.Server.Plugins
{
    public class PluginSettingsManager
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<string, PluginData> _pluginSettings;

        public PluginSettingsManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _pluginSettings = new Dictionary<string, PluginData>();
        }

        public PluginData GetPluginSettings(string pluginKey)
        {
            if (_pluginSettings.ContainsKey(pluginKey))
                return _pluginSettings[pluginKey];

            using (var scope = _serviceProvider.CreateScope())
            {
                var pluginRepository = scope.ServiceProvider.GetService<IRepository<Plugin>>();

                var pluginSettingsRepository = scope.ServiceProvider.GetService<IRepository<PluginSettings>>();

                var pluginExists = pluginRepository.TableNoTracking.Any(plugin => plugin.Key == pluginKey);

                if (!pluginExists)
                    return null;

                var pluginDataJson = pluginSettingsRepository.TableNoTracking.FirstOrDefault(pluginSettings =>
                    pluginSettings.PluginKey == pluginKey)?.Data;

                if (string.IsNullOrEmpty(pluginDataJson))
                {
                    _pluginSettings[pluginKey] = new PluginData();
                }
                else
                {
                    _pluginSettings[pluginKey] = JsonConvert.DeserializeObject<PluginData>(pluginDataJson);
                }
            }

            return _pluginSettings[pluginKey];
        }

        public bool SetPluginSettings(string pluginKey, PluginData pluginSettings)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var pluginRepository = scope.ServiceProvider.GetService<IRepository<Plugin>>();
                var pluginSettingsRepository = scope.ServiceProvider.GetService<IRepository<PluginSettings>>();

                var pluginExists = pluginRepository.TableNoTracking.Any(plugin => plugin.Key == pluginKey);

                if (!pluginExists)
                    return false;

                var oldPluginSettings = pluginSettingsRepository.Table.FirstOrDefault(pluginSettings1 =>
                    pluginSettings1.PluginKey == pluginKey) ?? new PluginSettings
                {
                    PluginKey = pluginKey
                };

                oldPluginSettings.Data = JsonConvert.SerializeObject(pluginSettings);

                pluginSettingsRepository.Update(oldPluginSettings);
            }

            _pluginSettings[pluginKey].Data = pluginSettings.Data;

            return true;
        }
    }
}

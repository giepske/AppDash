using System;
using System.Collections.Generic;
using System.Linq;
using AppDash.Plugins;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

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
                var pluginSettingsRepository = scope.ServiceProvider.GetService<IRepository<PluginSettings>>();

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

            Console.WriteLine("GetPluginSettings: " + _pluginSettings[pluginKey].GetHashCode());
            return _pluginSettings[pluginKey];
        }

        public void SetPluginSettings(string pluginKey, PluginData pluginSettings)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var pluginSettingsRepository = scope.ServiceProvider.GetService<IRepository<PluginSettings>>();

                var oldPluginSettings = pluginSettingsRepository.Table.FirstOrDefault(pluginSettings =>
                    pluginSettings.PluginKey == pluginKey);

                if(oldPluginSettings == null)
                    oldPluginSettings = new PluginSettings
                    {
                        PluginKey = pluginKey
                    };

                oldPluginSettings.Data = JsonConvert.SerializeObject(pluginSettings);

                pluginSettingsRepository.Update(oldPluginSettings);
            }

            Console.WriteLine("SetPluginSettings: " + _pluginSettings[pluginKey].GetHashCode());
            _pluginSettings[pluginKey].Data = pluginSettings.Data;
        }
    }
}

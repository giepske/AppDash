using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Plugins;

namespace AppDash.Client.Plugins
{
    public class PluginSettingsManager
    {
        private readonly HttpClient _httpClient;

        private readonly Dictionary<string, PluginData> _pluginSettings;

        public PluginSettingsManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _pluginSettings = new Dictionary<string, PluginData>();
        }

        public async Task<PluginData> GetPluginSettings(string pluginKey)
        {
            if (_pluginSettings.ContainsKey(pluginKey))
                return _pluginSettings[pluginKey];

            var result = await _httpClient.GetJson<ApiResult>($"api/plugins/{pluginKey}/settings");

            var settingsData = result.GetData<PluginData>();

            _pluginSettings[pluginKey] = settingsData;

            return _pluginSettings[pluginKey];
        }
    }
}

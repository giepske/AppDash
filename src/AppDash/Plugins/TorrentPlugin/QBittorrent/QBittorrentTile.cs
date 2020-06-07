using System;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using Newtonsoft.Json.Linq;

namespace TorrentPlugin.QBittorrent
{
    public class QBittorrentTile : UpdatingPluginTile<QBitTorrentPlugin, QBittorrentTileComponent>
    {
        public QBittorrentTile(QBitTorrentPlugin plugin) : base(plugin, TimeSpan.FromSeconds(5))
        { }

        public override async Task<PluginData> OnUpdateDataRequest()
        {
            var pluginData = new PluginData();

            string host = PluginSettings.GetData<string>("Host");
            string loginToken = PluginSettings.GetData<string>("Cookie");

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(loginToken))
            {
                pluginData.SetData("IsSuccess", false);
            }
            else
            {
                string result = await QBittorrentHelper.GetCurrentStats(host, "");

                if (string.IsNullOrEmpty(result))
                {
                    pluginData.SetData("IsSuccess", false);
                }
                else
                {
                    JObject jObject = JObject.Parse(result);

                    pluginData.SetData("CurrentDownload", jObject["dl_info_speed"].Value<int>());
                    pluginData.SetData("CurrentUpload", jObject["up_info_speed"].Value<int>());
                    pluginData.SetData("IsSuccess", true);
                }
            }

            return pluginData;
        }
    }
}
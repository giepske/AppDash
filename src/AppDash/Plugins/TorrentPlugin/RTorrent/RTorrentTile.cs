using System;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;

namespace TorrentPlugin.RTorrent
{
    public class RTorrentTile : UpdatingPluginTile<RTorrentPlugin, RTorrentTileComponent>
    {
        public RTorrentTile(RTorrentPlugin plugin) : base(plugin, TimeSpan.FromSeconds(5))
        { }

        public override Task<PluginData> OnUpdateDataRequest()
        {
            var pluginData = new PluginData();

            string host = PluginSettings.GetData<string>("Host");
            int port = PluginSettings.GetData<int>("Port");

            if (string.IsNullOrEmpty(host) || port < 255 || port > 65535)
            {
                pluginData.SetData("IsSuccess", false);
            }
            else
            {
                var result = RTorrentHelper.GetCurrentStats(host, port);

                if (result == null)
                {
                    pluginData.SetData("IsSuccess", false);
                }
                else
                {
                    pluginData.SetData("CurrentDownload", result.Item1);
                    pluginData.SetData("CurrentUpload", result.Item2);
                    pluginData.SetData("IsSuccess", true);
                }
            }

            return Task.FromResult(pluginData);
        }
    }
}
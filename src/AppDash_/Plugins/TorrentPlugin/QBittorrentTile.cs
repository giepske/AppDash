using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;

namespace TorrentPlugin
{
    public class QBittorrentTile : UpdatingTile<QBitTorrentPlugin, QBittorrentComponent>
    {
        public QBittorrentTile(QBitTorrentPlugin plugin) : base(plugin, TimeSpan.FromSeconds(5))
        {
        }

        public override Task<PluginData> OnUpdateDataRequest()
        {
            return Task.FromResult(new PluginData(new KeyValuePair<string, object>("datetime", DateTime.Now)));
        }
    }
}
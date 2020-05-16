using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;

namespace TorrentPlugin
{
    //public class RTorrentPage : UpdatingPage<RTorrentPlugin, RTorrentComponent>
    //{
    //    public RTorrentPage(RTorrentPlugin plugin) : base(plugin, TimeSpan.FromSeconds(1))
    //    {
    //    }

    //    public override Task<PluginData> OnUpdateDataRequest()
    //    {
    //        return Task.FromResult(new PluginData(new KeyValuePair<string, object>("datetime", DateTime.Now)));
    //    }
    //}

    public class RTorrentTile : UpdatingTile<RTorrentPlugin, RTorrentComponent>
    {
        public RTorrentTile(RTorrentPlugin plugin) : base(plugin, TimeSpan.FromSeconds(1))
        {
        }

        public override Task<PluginData> OnUpdateDataRequest()
        {
            return Task.FromResult(new PluginData(new KeyValuePair<string, object>("datetime", DateTime.Now)));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Pages;

namespace TorrentPlugin.RTorrent
{
    public class RTorrentPage : UpdatingPluginPage<RTorrentPlugin, RTorrentPageComponent>
    {
        public RTorrentPage(RTorrentPlugin plugin) : base(plugin, TimeSpan.FromMinutes(1))
        {
        }

        public override Task<PluginData> OnUpdateDataRequest()
        {
            return Task.FromResult(new PluginData(new KeyValuePair<string, object>("datetime", DateTime.Now)));
        }
    }
}
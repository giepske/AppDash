using System;
using System.Collections.Generic;
using System.Text.Json;
using AppDash.Plugins;
using AppDash.Server.Core.Domain.Plugins;

namespace TorrentPlugin
{
    public class RTorrentPlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "rTorrent";
        public override string Icon { get; set; } = "rtorrent.png";

        public static void Main(string[] args)
        {

        }
    }
}
using AppDash.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace TorrentPlugin.QBittorrent
{
    public class QBitTorrentPlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "qBittorrent";
        public override string Description { get; set; } = "A plugin for viewing info about your qBittorrent client.";
        public override string Icon { get; set; } = "qbittorrent.svg";

        public override void ConfigureServices(IServiceCollection services)
        {
            
        }

        public static void Main(string[] args)
        {

        }
    }
}
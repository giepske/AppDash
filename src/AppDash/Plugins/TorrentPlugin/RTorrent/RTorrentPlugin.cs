using AppDash.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace TorrentPlugin.RTorrent
{
    public class RTorrentPlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "rTorrent";
        public override string Description { get; set; } = "A plugin for viewing info about your rTorrent client.";
        public override string Icon { get; set; } = "rtorrent.png";

        public override void ConfigureServices(IServiceCollection services)
        {
            
        }

        public static void Main(string[] args)
        {

        }
    }
}
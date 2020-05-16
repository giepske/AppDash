using AppDash.Plugins;

namespace TorrentPlugin
{
    public class QBitTorrentPlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "qBittorrent";
        public override string Icon { get; set; } = "qbittorrent.svg";

        public static void Main(string[] args)
        {

        }
    }
}
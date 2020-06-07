using AppDash.Plugins.Settings;

namespace TorrentPlugin.QBittorrent
{
    public class Settings : PluginSettings<QBitTorrentPlugin, SettingsComponent>
    {
        public Settings(QBitTorrentPlugin plugin) : base(plugin)
        { }
    }
}
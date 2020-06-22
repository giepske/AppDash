using AppDash.Plugins.Settings;

namespace TorrentPlugin.RTorrent
{
    public class Settings : PluginSettings<RTorrentPlugin, SettingsComponent>
    {
        public Settings(RTorrentPlugin plugin) : base(plugin)
        { }
    }
}
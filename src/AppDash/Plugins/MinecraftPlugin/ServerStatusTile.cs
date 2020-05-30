using System;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;

namespace MinecraftPlugin
{
    public class ServerStatusTile : UpdatingPluginTile<MinecraftPlugin, ServerStatusTileComponent>
    {
        public ServerStatusTile(MinecraftPlugin plugin) : base(plugin, TimeSpan.FromMinutes(1))
        {

        }

        public override Task OnAfterLoad()
        {
            PingServer();

            return Task.CompletedTask;
        }

        public override Task<PluginData> OnUpdateDataRequest()
        {
            PingServer();

            return Task.FromResult(CachedData);
        }

        private void PingServer()
        {
            string host = PluginSettings.GetData<string>("Host");
            int port = PluginSettings.GetData<int>("Port");

            if (string.IsNullOrEmpty(host) || port == 0)
            {
                CachedData.RemoveKey("ServerStatus");
            }
            else if (Uri.CheckHostName(host) == UriHostNameType.Unknown)
            {
                CachedData.RemoveKey("ServerStatus");
            }
            else
            {
                CachedData.SetData("ServerStatus", new MinecraftServerHelper().PingServer(host, port));
            }
        }
    }
}
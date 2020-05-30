using System;
using System.Linq;
using AppDash.Core;
using AppDash.Server.Core.Communication;
using Microsoft.AspNetCore.SignalR;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A tile that can be updated in real time.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this tile belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PluginTileComponent"/> this tile belongs to.</typeparam>
    public abstract class RealtimePluginTile<TPlugin, TRazorComponent> : PluginTile<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PluginTileComponent
    {
        private IHubContext<ChatHub> _hubContext;
        private PermissionMemoryCache _permissionMemoryCache;

        protected RealtimePluginTile(TPlugin plugin) : base(plugin, PluginTileType.RealtimeData, TimeSpan.MinValue, false)
        {

        }

        private void SetDependencies(IHubContext<ChatHub> hubContext, PermissionMemoryCache permissionMemoryCache)
        {
            _hubContext = hubContext;
            _permissionMemoryCache = permissionMemoryCache;
        }

        internal override void OnUpdateData(PluginData pluginData)
        {
            //TODO fix permissions, now it doesn't use permissions at all
            var clients = _hubContext.Clients.Clients(_permissionMemoryCache.GetClients(null).ToList());

            clients.SendAsync("UpdateTileData", RazorComponentType.FullName, pluginData).Wait();
        }
    }
}
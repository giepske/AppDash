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
    /// <typeparam name="TPlugin">The derived AppDashPlugin this tile belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived TileComponent this tile belongs to.</typeparam>
    public abstract class RealtimeTile<TPlugin, TRazorComponent> : Tile<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : TileComponent
    {
        private IHubContext<ChatHub> _hubContext;
        private PermissionMemoryCache _permissionMemoryCache;

        protected RealtimeTile(TPlugin plugin) : base(plugin, TileType.RealtimeData, TimeSpan.MinValue, false)
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
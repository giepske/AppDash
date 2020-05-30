using System;
using System.Linq;
using System.Threading;
using AppDash.Core;
using AppDash.Server.Core.Communication;
using Microsoft.AspNetCore.SignalR;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A tile that will be updated after a certain interval.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this tile belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PluginTileComponent"/> this tile belongs to.</typeparam>
    public abstract class UpdatingPluginTile<TPlugin, TRazorComponent> : PluginTile<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PluginTileComponent
    {
        private Timer _backgroundThreadTimer;
        private IHubContext<ChatHub> _hubContext;
        private PermissionMemoryCache _permissionMemoryCache;

        protected UpdatingPluginTile(TPlugin plugin, TimeSpan updateInterval) : base(plugin, PluginTileType.UpdatingData, updateInterval, false)
        {
            _backgroundThreadTimer = new Timer(Callback, null, 0, (int)updateInterval.TotalMilliseconds);
        }

        private void Callback(object state)
        {
            //dont run callback if dependencies are not yet set.
            if (_hubContext == null)
                return;

            var pluginData = OnUpdateDataRequest().Result;

            if (pluginData != null)
            {
                UpdateData(pluginData);
            }
        }

        public void SetDependencies(IHubContext<ChatHub> hubContext, PermissionMemoryCache permissionMemoryCache)
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

        public override void Dispose()
        {
            _backgroundThreadTimer.Dispose();
        }
    }
}
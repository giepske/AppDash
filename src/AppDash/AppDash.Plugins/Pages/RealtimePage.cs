using System;
using System.Linq;
using AppDash.Core;
using AppDash.Server.Core.Communication;
using Microsoft.AspNetCore.SignalR;

namespace AppDash.Plugins.Pages
{
    /// <summary>
    /// A page that can be updated in real time.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this page belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PageComponent"/> this page belongs to.</typeparam>
    public abstract class RealtimePage<TPlugin, TRazorComponent> : Page<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PageComponent
    {
        private IHubContext<ChatHub> _hubContext;
        private PermissionMemoryCache _permissionMemoryCache;

        protected RealtimePage(TPlugin plugin) : base(plugin, PageType.RealtimeData, TimeSpan.MinValue, false)
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
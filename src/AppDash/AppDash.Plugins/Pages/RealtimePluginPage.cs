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
    /// <typeparam name="TRazorComponent">The derived <see cref="PluginPageComponent"/> this page belongs to.</typeparam>
    public abstract class RealtimePluginPage<TPlugin, TRazorComponent> : PluginPage<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PluginPageComponent
    {
        private IHubContext<ChatHub> _hubContext;
        private PermissionMemoryCache _permissionMemoryCache;

        protected RealtimePluginPage(TPlugin plugin) : base(plugin, PluginPageType.RealtimeData, TimeSpan.MinValue, false)
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

            clients.SendAsync("UpdatePageData", PluginKey, RazorComponentType.FullName, pluginData).Wait();
        }
    }
}
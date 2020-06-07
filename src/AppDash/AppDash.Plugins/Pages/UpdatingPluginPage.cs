using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Server.Core.Communication;
using Microsoft.AspNetCore.SignalR;

namespace AppDash.Plugins.Pages
{
    /// <summary>
    /// A page that will be updated after a certain interval.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this page belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PluginPageComponent"/> this page belongs to.</typeparam>
    public abstract class UpdatingPluginPage<TPlugin, TRazorComponent> : PluginPage<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PluginPageComponent
    {
        private readonly TimeSpan _updateInterval;
        private Timer _backgroundThreadTimer;
        private IHubContext<ChatHub> _hubContext;
        private PermissionMemoryCache _permissionMemoryCache;

        protected UpdatingPluginPage(TPlugin plugin, TimeSpan updateInterval) : base(plugin, PluginPageType.UpdatingData, updateInterval, false)
        {
            _updateInterval = updateInterval;
        }

        public override Task OnAfterLoad()
        {
            _backgroundThreadTimer = new Timer(Callback, null, 200, (int)_updateInterval.TotalMilliseconds);

            return Task.CompletedTask;
        }

        private void Callback(object state)
        {
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

            clients.SendAsync("UpdatePageData", PluginKey, RazorComponentType.FullName, pluginData).Wait();
        }

        public override void Dispose()
        {
            _backgroundThreadTimer.Dispose();
        }
    }
}
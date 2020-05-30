using System;
using System.Threading.Tasks;

namespace AppDash.Plugins.Pages
{
    public abstract class PluginPage<TPlugin, TRazorComponent> : IDisposable, IPage where TPlugin : AppDashPlugin where TRazorComponent : PluginPageComponent
    {
        /// <summary>
        /// The type of this page.
        /// </summary>
        public PluginPageType PluginPageType { get; }
        /// <summary>
        /// Whether this page is refreshable or not.
        /// <para>
        /// Only used when <see cref="PluginPageType"/> is <see cref="PluginPageType.StaticData"/>. Will be <see langword="false"/> otherwise.
        /// </para>
        /// </summary>
        public bool Refreshable { get; }
        /// <summary>
        /// The url that the page should go to.
        /// <para>
        /// Only used when <see cref="PluginPageType"/> is <see cref="PluginPageType.Link"/>. Will be <see langword="null"/> otherwise.
        /// </para>
        /// </summary>
        public string Url { get; }
        /// <summary>
        /// The interval between when data gets updated.
        /// <para>
        /// Only used when <see cref="PluginPageType"/> is <see cref="PluginPageType.UpdatingData"/>. Will be <see cref="TimeSpan.MinValue"/> otherwise.
        /// </para>
        /// </summary>
        public TimeSpan UpdateInterval { get; }
        /// <summary>
        /// Cached PluginData that will be set whenever <see cref="UpdateData"/> gets called.
        /// <para>
        /// This should to be set when <see cref="OnAfterLoad"/> gets called.
        /// </para>
        /// </summary>
        public PluginData CachedData { get; protected set; }
        /// <summary>
        /// The plugin this page belongs to.
        /// </summary>
        public TPlugin Plugin { get; }
        /// <summary>
        /// The Type of the razor component that belongs to this page.
        /// </summary>
        public Type RazorComponentType { get; }

        protected PluginPage(TPlugin plugin, PluginPageType pluginPageType, TimeSpan updateInterval, bool refreshable, string url = null)
        {
            Plugin = plugin;
            RazorComponentType = typeof(TRazorComponent);
            PluginPageType = pluginPageType;
            UpdateInterval = updateInterval;
            Refreshable = refreshable;
            Url = url;
        }

        /// <summary>
        /// Will be called after the plugin gets loaded, you can set <see cref="CachedData"/> and start your background threads here.
        /// </summary>
        /// <returns></returns>
        public virtual Task OnAfterLoad()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Should be called if the page needs to be updated.
        /// <para>
        /// This method should be called manually.
        /// </para>
        /// <para>
        /// This method will also set <see cref="CachedData"/>.
        /// </para>
        /// </summary>
        /// <param name="pluginData">The data the page will receive.</param>
        /// <returns></returns>
        public void UpdateData(PluginData pluginData)
        {
            CachedData = pluginData;

            OnUpdateData(pluginData);
        }

        /// <summary>
        /// Will send updated data to the connected clients.
        /// <para>
        /// This only needs to be implemented when <see cref="PluginPageType"/> is <c>PageType.RealtimeData</c>.
        /// </para>
        /// </summary>
        /// <param name="pluginData">The data the page will receive.</param>
        /// <returns></returns>
        internal virtual void OnUpdateData(PluginData pluginData)
        { }

        /// <summary>
        /// Will be called when the page needs to be updated, based on the value of <see cref="UpdateInterval"/>.
        /// <para>
        /// This method will be called for you and should not be called manually.
        /// </para>
        /// <para>
        /// This only needs to be implemented when <see cref="PluginPageType"/> is <c>PageType.UpdatingData</c>.
        /// </para>
        /// </summary>
        /// <returns>Return a PluginData object to update the page or <see langword="null"/> to not update anything.</returns>
        public virtual Task<PluginData> OnUpdateDataRequest() => null;

        /// <summary>
        /// Will be called when the page gets unloaded, dispose your background threads here.
        /// </summary>
        public abstract void Dispose();
    }
}

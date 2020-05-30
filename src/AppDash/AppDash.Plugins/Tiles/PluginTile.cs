using System;
using System.Threading.Tasks;

namespace AppDash.Plugins.Tiles
{
    public abstract class PluginTile<TPlugin, TRazorComponent> : IDisposable, ITile where TPlugin : AppDashPlugin where TRazorComponent : PluginTileComponent
    {
        /// <summary>
        /// The type of this tile.
        /// </summary>
        public PluginTileType PluginTileType { get; }
        /// <summary>
        /// Whether this tile is refreshable or not.
        /// <para>
        /// Only used when <see cref="PluginTileType"/> is <see cref="PluginTileType.StaticData"/>. Will be <see langword="false"/> otherwise.
        /// </para>
        /// </summary>
        public bool Refreshable { get; }
        /// <summary>
        /// The url that the tile should go to.
        /// <para>
        /// Only used when <see cref="PluginTileType"/> is <see cref="PluginTileType.Link"/>. Will be <see langword="null"/> otherwise.
        /// </para>
        /// </summary>
        public string Url { get; }
        /// <summary>
        /// The interval between when data gets updated.
        /// <para>
        /// Only used when <see cref="PluginTileType"/> is <see cref="PluginTileType.UpdatingData"/>. Will be <see cref="TimeSpan.MinValue"/> otherwise.
        /// </para>
        /// </summary>
        public TimeSpan UpdateInterval { get; }
        /// <summary>
        /// Cached data that will be set whenever <see cref="UpdateData"/> gets called. Cached data here will be reset whenever the plugin restarts.
        /// <para>
        /// This should to be set when <see cref="OnAfterLoad"/> gets called.
        /// </para>
        /// </summary>
        public PluginData CachedData { get; protected set; } = new PluginData();
        /// <summary>
        /// The plugin settings. Data should not be set here because it will not be saved.
        /// </summary>
        public PluginData PluginSettings { get; protected set; }
        /// <summary>
        /// The plugin this tile belongs to.
        /// </summary>
        public TPlugin Plugin { get; }
        /// <summary>
        /// The Type of the razor component that belongs to this tile.
        /// </summary>
        public Type RazorComponentType { get; }

        protected PluginTile(TPlugin plugin, PluginTileType pluginTileType, TimeSpan updateInterval, bool refreshable, string url = null)
        {
            Plugin = plugin;
            RazorComponentType = typeof(TRazorComponent);
            PluginTileType = pluginTileType;
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
        /// Should be called if the tile needs to be updated.
        /// <para>
        /// This method should be called manually.
        /// </para>
        /// <para>
        /// This method will also set <see cref="CachedData"/> to the <paramref name="pluginData"/> value.
        /// If you want to keep old data you need to update and return the old <see cref="CachedData"/> object.
        /// </para>
        /// </summary>
        /// <param name="pluginData">The data the tile will receive.</param>
        /// <returns></returns>
        public void UpdateData(PluginData pluginData)
        {
            CachedData = pluginData;

            OnUpdateData(pluginData);
        }

        /// <summary>
        /// Will send updated data to the connected clients.
        /// <para>
        /// This only needs to be implemented when <see cref="TileType"/> is <c>TileType.RealtimeData</c>.
        /// </para>
        /// </summary>
        /// <param name="pluginData">The data the tile will receive.</param>
        /// <returns></returns>
        internal virtual void OnUpdateData(PluginData pluginData)
        { }

        /// <summary>
        /// Will be called when the tile needs to be updated, based on the value of <see cref="UpdateInterval"/>.
        /// <para>
        /// This method will be called for you and should not be called manually.
        /// </para>
        /// <para>
        /// This only needs to be implemented when <see cref="TileType"/> is <c>TileType.UpdatingData</c>.
        /// </para>
        /// </summary>
        /// <returns>Return a PluginData object to update the tile or <see langword="null"/> to not update anything.</returns>
        public virtual Task<PluginData> OnUpdateDataRequest() => null;

        /// <summary>
        /// Will be called when the tile gets unloaded, dispose your background threads here.
        /// </summary>
        public abstract void Dispose();
    }
}

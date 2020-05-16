using System;
using System.Threading.Tasks;

namespace AppDash.Plugins.Tiles
{
    public abstract class Tile<TPlugin, TRazorComponent> : IDisposable, ITile where TPlugin : AppDashPlugin where TRazorComponent : TileComponent
    {
        /// <summary>
        /// The type of this tile.
        /// </summary>
        public TileType TileType { get; }
        /// <summary>
        /// Whether this tile is refreshable or not.
        /// <para>
        /// Only used when <see cref="TileType"/> is <see cref="TileType.StaticData"/>. Will be <see langword="false"/> otherwise.
        /// </para>
        /// </summary>
        public bool Refreshable { get; }
        /// <summary>
        /// The url that the tile should go to.
        /// <para>
        /// Only used when <see cref="TileType"/> is <see cref="TileType.Link"/>. Will be <see langword="null"/> otherwise.
        /// </para>
        /// </summary>
        public string Url { get; }
        /// <summary>
        /// The interval between when data gets updated.
        /// <para>
        /// Only used when <see cref="TileType"/> is <see cref="TileType.UpdatingData"/>. Will be <see cref="TimeSpan.MinValue"/> otherwise.
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
        /// The plugin this tile belongs to.
        /// </summary>
        public TPlugin Plugin { get; }
        /// <summary>
        /// The Type of the razor component that belongs to this tile.
        /// </summary>
        public Type RazorComponentType { get; }

        protected Tile(TPlugin plugin, TileType tileType, TimeSpan updateInterval, bool refreshable, string url = null)
        {
            Plugin = plugin;
            RazorComponentType = typeof(TRazorComponent);
            TileType = tileType;
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
        /// This method will also set <see cref="CachedData"/>.
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

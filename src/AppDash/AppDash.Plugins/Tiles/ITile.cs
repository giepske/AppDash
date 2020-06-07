using System;
using System.Threading.Tasks;

namespace AppDash.Plugins.Tiles
{
    public interface ITile
    {
        PluginTileType PluginTileType { get; }
        bool Refreshable { get; }
        string Url { get; }
        TimeSpan UpdateInterval { get; }
        PluginData CachedData { get; }
        PluginData PluginSettings { get; }
        string PluginKey { get; set; }
        string TileKey { get; set; }

        /// <summary>
        /// Will be called after the plugin gets loaded. Do not yet communicate with clients here.
        /// </summary>
        /// <returns></returns>
        Task OnAfterLoad();

        /// <summary>
        /// Will be called after <see cref="OnAfterLoad"/>, if needed this can set background threads.
        /// </summary>
        /// <returns></returns>
        Task OnAfterLoadInternal();

        /// <summary>
        /// Should be called if the tile needs to be updated.
        /// <para>
        /// This method should be called manually.
        /// </para>
        /// </summary>
        /// <param name="pluginData">The data the tile will receive.</param>
        /// <returns></returns>
        void UpdateData(PluginData pluginData);

        /// <summary>
        /// Will be called when the tile needs to be updated, based on the value of <see cref="Tile{TPlugin,TRazorComponent}.UpdateInterval"/>.
        /// <para>
        /// This method will be called for you and should not be called manually.
        /// </para>
        /// <para>
        /// This only needs to be implemented when <see cref="Tile{TPlugin,TRazorComponent}.TileType"/> is <c>TileType.UpdatingContent</c>.
        /// </para>
        /// </summary>
        /// <returns>Return a PluginData object to update the tile or <see langword="null"/> to not update anything.</returns>
        Task<PluginData> OnUpdateDataRequest();
    }
}
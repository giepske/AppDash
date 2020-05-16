using System;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A tile that will contain static data that will not be automatically updated.
    /// </summary>
    /// <typeparam name="TPlugin">The derived AppDashPlugin this tile belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived TileComponent this tile belongs to.</typeparam>
    public abstract class StaticTile<TPlugin, TRazorComponent> : Tile<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : TileComponent
    {
        protected StaticTile(TPlugin plugin, bool refreshable) : base(plugin, TileType.StaticData, TimeSpan.Zero, refreshable)
        {

        }
    }
}
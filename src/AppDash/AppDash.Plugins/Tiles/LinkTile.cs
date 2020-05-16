using System;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A tile that simply contains a link to a url.
    /// </summary>
    /// <typeparam name="TPlugin">The derived AppDashPlugin this tile belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived TileComponent this tile belongs to.</typeparam>
    public abstract class LinkTile<TPlugin, TRazorComponent> : Tile<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : TileComponent, IDisposable
    {
        protected LinkTile(TPlugin plugin, string url) : base(plugin, TileType.Link, TimeSpan.Zero, false, url)
        {

        }
    }
}
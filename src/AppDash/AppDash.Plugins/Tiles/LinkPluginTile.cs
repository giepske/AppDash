using System;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A tile that simply contains a link to a url.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this tile belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PluginTileComponent"/> this tile belongs to.</typeparam>
    public abstract class LinkPluginTile<TPlugin, TRazorComponent> : PluginTile<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PluginTileComponent, IDisposable
    {
        protected LinkPluginTile(TPlugin plugin, string url) : base(plugin, PluginTileType.Link, TimeSpan.Zero, false, url)
        {

        }
    }
}
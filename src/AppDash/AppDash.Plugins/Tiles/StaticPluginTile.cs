using System;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A tile that will contain static data that will not be automatically updated.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this tile belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PluginTileComponent"/> this tile belongs to.</typeparam>
    public abstract class StaticPluginTile<TPlugin, TRazorComponent> : PluginTile<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PluginTileComponent
    {
        protected StaticPluginTile(TPlugin plugin, bool refreshable) : base(plugin, PluginTileType.StaticData, TimeSpan.Zero, refreshable)
        {

        }
    }
}
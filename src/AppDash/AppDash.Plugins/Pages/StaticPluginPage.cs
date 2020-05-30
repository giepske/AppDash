using System;

namespace AppDash.Plugins.Pages
{
    /// <summary>
    /// A page that will contain static data that will not be automatically updated.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this page belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PluginPageComponent"/> this page belongs to.</typeparam>
    public abstract class StaticPluginPage<TPlugin, TRazorComponent> : PluginPage<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PluginPageComponent
    {
        protected StaticPluginPage(TPlugin plugin, bool refreshable) : base(plugin, PluginPageType.StaticData, TimeSpan.Zero, refreshable)
        {

        }
    }
}
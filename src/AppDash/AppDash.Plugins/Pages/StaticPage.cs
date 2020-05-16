using System;

namespace AppDash.Plugins.Pages
{
    /// <summary>
    /// A page that will contain static data that will not be automatically updated.
    /// </summary>
    /// <typeparam name="TPlugin">The derived <see cref="AppDashPlugin"/> this page belongs to.</typeparam>
    /// <typeparam name="TRazorComponent">The derived <see cref="PageComponent"/> this page belongs to.</typeparam>
    public abstract class StaticPage<TPlugin, TRazorComponent> : Page<TPlugin, TRazorComponent> where TPlugin : AppDashPlugin where TRazorComponent : PageComponent
    {
        protected StaticPage(TPlugin plugin, bool refreshable) : base(plugin, PageType.StaticData, TimeSpan.Zero, refreshable)
        {

        }
    }
}
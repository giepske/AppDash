using Microsoft.AspNetCore.Components;

namespace AppDash.Plugins.Pages
{
    /// <summary>
    /// A Blazor page component.
    /// </summary>
    public abstract class PageComponent : ComponentBase
    {
        /// <summary>
        /// The relative URL path of the page, coming after the plugin key. The url has the following template: https://example.com/plugin/{PluginKey}/{RelativePath}
        /// </summary>
        public abstract string RelativePath { get; set; }

        /// <summary>
        /// Whether this page is the main page, the plugin's menu item will redirect to this page if set to <see langword="true"/>.
        /// </summary>
        public abstract bool IsMainPage { get; set; }

        /// <summary>
        /// Will be called when data is received, when the tile is first shown or when it receives updated data.
        /// <para>
        /// After this method is executed, <see cref="ComponentBase.StateHasChanged()"/> will be called if the return value is <see langword="true"/>.
        /// </para>
        /// <returns>
        /// Should return true if the component needs to be re-rendered.
        /// </returns>
        /// </summary>
        public abstract bool OnDataReceived(PluginData pluginData);

        /// <summary>
        /// Re-render the component to update HTML data on the screen.
        /// <para>
        /// This should not be called manually.
        /// </para>
        /// </summary>
        public new void StateHasChanged() => base.StateHasChanged();
    }
}
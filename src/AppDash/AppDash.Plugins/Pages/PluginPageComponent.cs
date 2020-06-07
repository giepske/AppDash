using Microsoft.AspNetCore.Components;

namespace AppDash.Plugins.Pages
{
    /// <summary>
    /// A Blazor page component.
    /// </summary>
    public abstract class PluginPageComponent : ComponentBase
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
        /// The plugin key of the plugin that belongs to this page.
        /// </summary>
        public string PluginKey { get; set; }

        /// <summary>
        /// The plugin settings of the plugin.
        /// </summary>
        public PluginData PluginSettings { get; set; }

        /// <summary>
        /// The data object with data from the corresponding <see cref="PluginPage{TPlugin,TRazorComponent}"/>.
        /// </summary>
        public PluginData Data { get; set; }

        /// <summary>
        /// Will be called after <see cref="Data"/> is updated, after the page is first shown or after it receives updated data.
        /// <para>
        /// After this method is executed, <see cref="ComponentBase.StateHasChanged()"/> will be called if the return value is <see langword="true"/>.
        /// </para>
        /// <returns>
        /// Should return true if the component needs to be re-rendered.
        /// </returns>
        /// </summary>
        public abstract bool OnUpdate();

        /// <summary>
        /// Re-render the component to update HTML data on the screen.
        /// <para>
        /// This should not be called manually.
        /// </para>
        /// </summary>
        public new void StateHasChanged() => base.StateHasChanged();
    }
}
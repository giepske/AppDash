using Microsoft.AspNetCore.Components;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A Blazor tile component.
    /// </summary>
    public abstract class PluginTileComponent : ComponentBase
    {
        /// <summary>
        /// If this tile should redirect to a url you should specify it here.
        /// </summary>
        public abstract string Url { get; set; }

        /// <summary>
        /// The plugin key of the plugin that belongs to this tile.
        /// </summary>
        public string PluginKey { get; set; }

        /// <summary>
        /// The plugin settings of the plugin.
        /// </summary>
        public PluginData PluginSettings { get; set; }

        /// <summary>
        /// The data object with data from the corresponding <see cref="PluginTile{TPlugin,TRazorComponent}"/>.
        /// </summary>
        public PluginData Data { get; set; }

        /// <summary>
        /// Will be called when <see cref="Data"/> is updated, after the tile is first shown or after it receives updated data.
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
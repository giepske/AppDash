using Microsoft.AspNetCore.Components;

namespace AppDash.Plugins.Tiles
{
    /// <summary>
    /// A Blazor tile component.
    /// </summary>
    public abstract class TileComponent : ComponentBase
    {
        /// <summary>
        /// If this tile should redirect to a url you should specify it here.
        /// </summary>
        public abstract string Url { get; set; }

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
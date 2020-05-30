using System.Net.Http;
using System.Threading.Tasks;
using AppDash.Core;
using Microsoft.AspNetCore.Components;

namespace AppDash.Plugins.Settings
{
    /// <summary>
    /// A Blazor settings component.
    /// </summary>
    public abstract class PluginSettingsComponent : ComponentBase
    {
        public HttpClient HttpClient { get; set; }

        [Parameter]
        public string PluginKey { get; set; }

        /// <summary>
        /// The PluginData object containing the plugin settings.
        /// </summary>
        public PluginData SettingsData;

        /// <summary>
        /// This will be called whenever the component has its data set and can be used for initialization.
        /// <para>
        /// Note: This method can be called often, don't put long running or high demanding code here.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public virtual Task Initialize() => Task.CompletedTask;

        /// <summary>
        /// Save <see cref="SettingsData"/> to the server.
        /// </summary>
        /// <returns></returns>
        public async Task SaveSettingsData()
        {
            await HttpClient.PatchJson<ApiResult>($"api/plugins/{PluginKey}/settings", SettingsData);
        }

        /// <summary>
        /// Re-render the component to update HTML data on the screen.
        /// <para>
        /// This should not be called manually.
        /// </para>
        /// </summary>
        public new void StateHasChanged() => base.StateHasChanged();
    }
}
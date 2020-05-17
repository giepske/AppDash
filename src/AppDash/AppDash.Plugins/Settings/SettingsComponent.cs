using System;
using System.Net.Http;
using System.Threading.Tasks;
using AppDash.Core;
using Microsoft.AspNetCore.Components;

namespace AppDash.Plugins.Settings
{
    /// <summary>
    /// A Blazor settings component.
    /// </summary>
    public abstract class SettingsComponent : ComponentBase
    {
        public HttpClient HttpClient { get; set; }

        [Parameter]
        public string PluginKey { get; set; }

        /// <summary>
        /// The PluginData object containing the plugin settings.
        /// </summary>
        public PluginData SettingsData;

        public SettingsComponent()
        {
            Console.WriteLine("SettingsComponent " + GetType());
        }

        /// <summary>
        /// Save <see cref="SettingsData"/> to the server.
        /// </summary>
        /// <returns></returns>
        public async Task SaveSettingsData()
        {
             Console.WriteLine("wefewfwe" + HttpClient);
             Console.WriteLine("asdsadasd" + PluginKey);
             Console.WriteLine("wefeewrfwfwe" + SettingsData);

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
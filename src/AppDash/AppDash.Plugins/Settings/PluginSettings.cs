using System;
using System.Threading.Tasks;

namespace AppDash.Plugins.Settings
{
    public abstract class PluginSettings<TPlugin, TRazorComponent> : IDisposable, ISettings where TPlugin : AppDashPlugin where TRazorComponent : PluginSettingsComponent
    {
        /// <summary>
        /// Cached PluginData that will be set whenever <see cref="OnUpdateData"/> gets called.
        /// <para>
        /// This should to be set when <see cref="OnAfterLoad"/> gets called.
        /// </para>
        /// </summary>
        public PluginData SettingsData { get; protected set; }
        /// <summary>
        /// The plugin this page belongs to.
        /// </summary>
        public TPlugin Plugin { get; }
        /// <summary>
        /// The Type of the razor component that belongs to this settings page.
        /// </summary>
        public Type RazorComponentType { get; }

        protected PluginSettings(TPlugin plugin)
        {
            Plugin = plugin;
            RazorComponentType = typeof(TRazorComponent);
        }

        /// <summary>
        /// Will be called after the plugin gets loaded, you can modify <see cref="SettingsData"/> and start your background threads here.
        /// <para>
        /// If your <see cref="SettingsData"/> requires standard data, you can use this method to check if the data is set and set it if not.
        /// </para>
        /// </summary>
        /// <returns>Return <see langword="true"/> when you have modified <see cref="SettingsData"/> to save it to the database, <see langword="false"/> otherwise.</returns>
        public virtual Task<bool> OnAfterLoad() => Task.FromResult(false);

        /// <summary>
        /// Will send updated data to the connected clients.
        /// </summary>
        /// <param name="pluginData">The data the page will receive.</param>
        /// <returns></returns>
        internal virtual void OnUpdateData(PluginData pluginData)
        { }

        /// <summary>
        /// Will be called when the settings needs to be shown on a client. Optionally add some logic here before sending the settings data.
        /// <para>
        /// This method will be called for you and should not be called manually.
        /// </para>
        /// </summary>
        /// <returns>Return a PluginData object to send to the settings page.</returns>
        public virtual Task<PluginData> OnUpdateDataRequest(PluginData settingsData) => Task.FromResult(settingsData);

        /// <summary>
        /// Will be called when the settings page gets unloaded, dispose your background threads here.
        /// </summary>
        public virtual void Dispose()
        { }
    }
}

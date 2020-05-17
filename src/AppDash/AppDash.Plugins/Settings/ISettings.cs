using System;
using System.Threading.Tasks;

namespace AppDash.Plugins.Settings
{
    public interface ISettings
    {
        PluginData SettingsData { get; }

        /// <summary>
        /// Will be called after the plugin gets loaded, you can modify <see cref="SettingsData"/> and start your background threads here.
        /// <para>
        /// If your <see cref="SettingsData"/> requires standard data, you can use this method to check if the data is set and set it if not.
        /// </para>
        /// </summary>
        /// <returns>Return <see langword="true"/> when you have modified <see cref="SettingsData"/> to save it to the database, <see langword="false"/> otherwise.</returns>
        Task<bool> OnAfterLoad();

        ///// <summary>
        ///// Should be called if the page needs to be updated.
        ///// <para>
        ///// This method should be called manually.
        ///// </para>
        ///// </summary>
        ///// <param name="pluginData">The data the page will receive.</param>
        ///// <returns></returns>
        //void UpdateData(PluginData pluginData);

        /// <summary>
        /// Will be called when the settings needs to be shown on a client. Optionally add some logic here before sending the settings data.
        /// <para>
        /// This method will be called for you and should not be called manually.
        /// </para>
        /// </summary>
        /// <returns>Return a PluginData object to send to the settings page.</returns>
        Task<PluginData> OnUpdateDataRequest(PluginData settingsData);
    }
}
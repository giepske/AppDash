namespace AppDash.Plugins.Pages
{
    /// <summary>
    /// The type of page.
    /// </summary>
    public enum PluginPageType
    {
        /// <summary>
        /// Page contains static data that can optionally be manually refreshed.
        /// </summary>
        StaticData,
        /// <summary>
        /// Page contains data that gets updated on a given interval.
        /// </summary>
        UpdatingData,
        /// <summary>
        /// Page will receive updated content in real-time.
        /// </summary>
        RealtimeData
    }
}
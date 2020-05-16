namespace AppDash.Plugins
{
    public abstract class AppDashPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// The filename of the icon of the plugin.
        /// </summary>
        public abstract string Icon { get; set; }

        /// <summary>
        /// The unique key for the plugin, changing every server restart.
        /// <para>
        /// Note: This will be set by the server.
        /// </para>
        /// </summary>
        public string Key { get; set; }
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Plugins
{
    public abstract class AppDashPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// The description of the plugin.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// The filename of the icon of the plugin.
        /// </summary>
        public abstract string Icon { get; set; }

        /// <summary>
        /// The unique key for the plugin.
        /// <para>
        /// Note: This will be set by the server.
        /// </para>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The service provider for dependency injection.
        /// </summary>
        protected IServiceProvider ServiceProvider;

        /// <summary>
        /// Add services for dependency injection on PluginControllers.
        /// </summary>
        /// <param name="services"></param>
        public abstract void ConfigureServices(IServiceCollection services);
    }
}
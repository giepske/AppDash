using AppDash.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Plugins
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all plugin related dependencies to the IServiceCollection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddPlugins(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<PluginResolver>();
            serviceCollection.AddSingleton<PluginManager>();

            return serviceCollection;
        }

        /// <summary>
        /// Adds all the dependencies that plugins can inject. This method is called on the IServiceCollection of PluginManager when the IServiceProvider gets created.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddDependencies(this IServiceCollection serviceCollection)
        {


            return serviceCollection;
        }
    }
}

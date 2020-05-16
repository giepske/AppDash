using System;
using AppDash.Core;
using AppDash.Server.Core.Communication;
using Microsoft.AspNetCore.SignalR;
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
        /// <param name="baseServiceProvider"></param>
        /// <returns></returns>
        public static IServiceCollection AddDependencies(this IServiceCollection serviceCollection, IServiceProvider baseServiceProvider)
        {
            serviceCollection.AddScoped(provider => baseServiceProvider.GetService<IHubContext<ChatHub>>());
            serviceCollection.AddSingleton(provider => baseServiceProvider.GetService<PermissionMemoryCache>());

            return serviceCollection;
        }

        /// <summary>
        /// Adds all services to the service collection.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <returns></returns>
        public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<PermissionMemoryCache>();
            serviceCollection.AddSingleton<FileMemoryService>();
            serviceCollection.AddSingleton<FileService>();

            return serviceCollection;
        }
    }
}

﻿using System;
using AppDash.Core;
using AppDash.Server.Core.Communication;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using AppDash.Server.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
            serviceCollection.AddSingleton(provider => baseServiceProvider.GetService<IHubContext<ChatHub>>());
            serviceCollection.AddSingleton(provider => baseServiceProvider.GetService<PermissionMemoryCache>());
            serviceCollection.AddScoped<IRepository<PluginSettings>, Repository<PluginSettings>>();
            serviceCollection.AddScoped<IRepository<Plugin>, Repository<Plugin>>();
            serviceCollection.AddDbContext<AppDashContext>();

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

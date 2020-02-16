using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppDash.Server.Plugins
{
    public static class HostExtensions
    {
        public static IHost LoadPlugins(this IHost host)
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "plugins"));

            PluginManager pluginManager = host.Services.GetService<PluginManager>();

            pluginManager.LoadPlugins(Path.Combine(AppContext.BaseDirectory, "plugins"));
            pluginManager.InitializePlugins();

            return host;
        }
    }
}
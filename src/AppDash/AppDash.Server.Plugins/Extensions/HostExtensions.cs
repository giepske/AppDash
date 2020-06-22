using System;
using System.IO;
using AppDash.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppDash.Server.Plugins.Extensions
{
    public static class HostExtensions
    {
        public static IHost LoadPlugins(this IHost host)
        {
            Directory.CreateDirectory(Path.Combine(Config.ApplicationDirectory, "plugins"));

            PluginManager pluginManager = host.Services.GetService<PluginManager>();

            Console.WriteLine(Path.Combine(Config.ApplicationDirectory, "plugins"));
            Console.WriteLine(Directory.GetFiles(Path.Combine(Config.ApplicationDirectory, "plugins")).Length);

            pluginManager.LoadPlugins(Path.Combine(Config.ApplicationDirectory, "plugins"));
            //pluginManager.InitializePlugins();

            return host;
        }
    }
}
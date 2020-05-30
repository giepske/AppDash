using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using AppDash.Plugins;
using AppDash.Plugins.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using TorrentPlugin;

namespace AppDash.Server.Plugins
{
    public static class HostExtensions
    {
        public static IHost LoadPlugins(this IHost host)
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "plugins"));

            PluginManager pluginManager = host.Services.GetService<PluginManager>();

            pluginManager.LoadPlugins(Path.Combine(AppContext.BaseDirectory, "plugins"));
            //pluginManager.InitializePlugins();

            return host;
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using AppDash.Client.Plugins;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Client.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton<PluginManager>();
            builder.Services.AddSingleton<PageResolver>();
            builder.Services.AddSingleton<PageManager>();
            builder.Services.AddSingleton<SettingsResolver>();
            builder.Services.AddSingleton<SettingsManager>();
            builder.Services.AddSingleton<TileResolver>();
            builder.Services.AddSingleton<TileManager>();
            builder.Services.AddSingleton<PluginResolver>();
            builder.Services.AddSingleton<PluginLoader>();
            builder.Services.AddSingleton<PluginSettingsManager>();
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            await builder.Build().RunAsync();
        }
    }
}

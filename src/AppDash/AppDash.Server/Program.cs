using AppDash.Server.Plugins;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AppDash.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .LoadPlugins()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}

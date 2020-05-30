using System.Linq;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using Microsoft.Extensions.DependencyInjection;

namespace MinecraftPlugin
{
    public class MinecraftPlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "Minecraft Plugin";
        public override string Description { get; set; } = "A plugin for viewing basic info about your Minecraft server.";
        public override string Icon { get; set; } = "minecraft.png";

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MinecraftServerHelper>();
        }

        public static void Main(string[] args)
        { }
    }
}

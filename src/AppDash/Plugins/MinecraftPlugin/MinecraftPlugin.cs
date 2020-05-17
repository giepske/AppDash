using System;
using AppDash.Plugins;
using AppDash.Plugins.Settings;

namespace MinecraftPlugin
{
    public class Settings : Settings<MinecraftPlugin, SettingsComponent>
    {
        public Settings(MinecraftPlugin plugin) : base(plugin)
        {
            Console.WriteLine("MinecraftPlugin.Settings loaded!");
        }
    }

    public class MinecraftPlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "Minecraft Plugin";
        public override string Icon { get; set; } = "minecraft.png";

        public static void Main(string[] args)
        {

        }
    }
}

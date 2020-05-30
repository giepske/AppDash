using System;
using AppDash.Plugins.Settings;

namespace MinecraftPlugin
{
    public class Settings : PluginSettings<MinecraftPlugin, SettingsComponent>
    {
        public Settings(MinecraftPlugin plugin) : base(plugin)
        {
            Console.WriteLine("MinecraftPlugin.Settings loaded!");
        }
    }
}
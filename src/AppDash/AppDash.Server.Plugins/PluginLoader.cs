using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AppDash.Plugins;
using AppDash.Plugins.Controllers;
using AppDash.Plugins.Settings;
using AppDash.Plugins.Tiles;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Plugins
{
    public class PluginLoader
    {
        private readonly PluginResolver _pluginResolver;

        private IServiceCollection _serviceCollection;

        public PluginLoader(PluginResolver pluginResolver, IServiceProvider serviceProvider)
        {
            _pluginResolver = pluginResolver;

            _serviceCollection = new ServiceCollection()
                .AddDependencies(serviceProvider);
        }

        public IServiceProvider LoadPlugins(string pluginPath)
        {
            _pluginResolver.ClearPlugins();

            List<string> plugins = Directory.GetDirectories(pluginPath)
                .Where(directory => File.Exists(Path.Combine(directory, Path.GetFileName((string?) directory) + ".dll")))
                .Select(directory => Path.Combine(directory, Path.GetFileName(directory) + ".dll")).ToList();

            foreach (string plugin in plugins)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(plugin);

                    LoadAppDashPlugins(assembly);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            Console.WriteLine($"Loaded {_pluginResolver.GetPlugins().Count} plugin(s).");

            return _serviceCollection.BuildServiceProvider();
        }

        private void LoadAppDashPlugins(Assembly assembly)
        {
            var pluginTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(AppDashPlugin)).ToList();

            foreach (Type pluginType in pluginTypes)
            {
                string pluginKey = _pluginResolver.AddPlugin(pluginType);

                _serviceCollection.AddSingleton(pluginType);
                _serviceCollection.AddSingleton(typeof(AppDashPlugin), serviceProvider => serviceProvider.GetService(pluginType));

                LoadPluginTiles(pluginKey, pluginType, assembly);
                LoadPluginSettings(pluginKey, pluginType, assembly);
                LoadPluginControllers(pluginKey, pluginType, assembly);
            }
        }

        private void LoadPluginSettings(string pluginKey, Type pluginType, Assembly assembly)
        {
            var pluginSettingsType = assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType.GetGenericTypeDefinition() == typeof(PluginSettings<,>) &&
                type.BaseType?.GenericTypeArguments.FirstOrDefault() == pluginType);

            //this plugin doesn't have a settings page.
            if (pluginSettingsType == null)
                return;

            _serviceCollection.AddSingleton(typeof(ISettings), pluginSettingsType);

            _pluginResolver.AddPluginSettings(pluginKey, pluginSettingsType);
        }

        private void LoadPluginTiles(string pluginKey, Type pluginType, Assembly assembly)
        {
            var pluginTileTypes = assembly.GetTypes().Where(type =>
                (type.BaseType?.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.BaseType.GetGenericTypeDefinition() == typeof(PluginTile<,>) &&
                type.BaseType?.GenericTypeArguments.FirstOrDefault() == pluginType).ToList();

            foreach (Type pluginTileType in pluginTileTypes)
            {
                _serviceCollection.AddSingleton(typeof(ITile), pluginTileType);
            }

            _pluginResolver.AddPluginTiles(pluginKey, pluginTileTypes);
        }

        private void LoadPluginControllers(string pluginKey, Type pluginType, Assembly assembly)
        {
            var pluginControllerTypes = assembly.GetTypes().Where(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.GetGenericTypeDefinition() == typeof(PluginController<>) &&
                type.BaseType?.GenericTypeArguments.FirstOrDefault() == pluginType).ToList();

            _pluginResolver.AddPluginControllers(pluginKey, pluginControllerTypes);
        }
    }
}
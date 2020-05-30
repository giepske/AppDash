using AppDash.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using AppDash.Plugins.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Plugins
{
    public class PluginResolver
    {
        // <PluginKey>, <AppDashPlugin Type>
        private Dictionary<string, Type> _plugins;
        // <PluginKey>, <IServiceProvider>
        private Dictionary<string, IServiceProvider> _serviceProviders;
        // <AppDashPlugin Type>, <Tile Types>
        private Dictionary<Type, List<Type>> _pluginTiles;
        // <AppDashPlugin Type>, <Settings Type>
        private Dictionary<Type, Type> _pluginSettings;
        // <PluginKey>, <List<PluginController Type>>
        private Dictionary<string, List<Type>> _pluginControllers;

        public PluginResolver()
        {
            _plugins = new Dictionary<string, Type>();
            _serviceProviders = new Dictionary<string, IServiceProvider>();
            _pluginTiles = new Dictionary<Type, List<Type>>();
            _pluginSettings = new Dictionary<Type, Type>();
            _pluginControllers = new Dictionary<string, List<Type>>();
        }

        public string AddPlugin(Type pluginType)
        {
            var guid = Guid.NewGuid().ToString("N");

            _plugins.Add(guid, pluginType);
            _pluginTiles[pluginType] = new List<Type>();

            return guid;
        }

        public Dictionary<string, Type> GetPlugins()
        {
            return _plugins;
        }

        public Type GetPlugin(string pluginKey)
        {
            if(_plugins.ContainsKey(pluginKey))
                return _plugins[pluginKey];

            return null;
        }

        public void ClearPlugins()
        {
            _plugins.Clear();
            _pluginTiles.Clear();
        }

        public void AddPluginTiles(string pluginKey, IEnumerable<Type> pluginTiles)
        {
            var plugin = GetPlugin(pluginKey);

            foreach (Type pluginTile in pluginTiles)
            {
                _pluginTiles[plugin].Add(pluginTile);
            }
        }

        public List<Type> GetPluginTiles(string pluginKey)
        {
            return _pluginTiles[_plugins[pluginKey]];
        }

        public void AddPluginSettings(string pluginKey, Type pluginSettingsType)
        {
            var plugin = GetPlugin(pluginKey);

            _pluginSettings[plugin] = pluginSettingsType;
        }

        public Type GetPluginSettings(string pluginKey)
        {
            if (!_plugins.ContainsKey(pluginKey))
                return null;

            var plugin = _plugins[pluginKey];
            
            if(_pluginSettings.ContainsKey(plugin))
                return _pluginSettings[plugin];

            return null;
        }

        public void SetPluginKey(string oldPluginKey, string newPluginKey)
        {
            var plugin = _plugins[oldPluginKey];
            _plugins[newPluginKey] = plugin;
            _plugins.Remove(oldPluginKey);

            var controllers = _pluginControllers[oldPluginKey];
            _pluginControllers[newPluginKey] = controllers;
            _pluginControllers.Remove(oldPluginKey);
        }

        public string GetPluginKey(AppDashPlugin pluginInstance)
        {
            var type = pluginInstance.GetType();

            return _plugins.FirstOrDefault(plugin => plugin.Value == type).Key;
        }

        public void AddPluginControllers(string pluginKey, List<Type> pluginControllerTypes)
        {
            _pluginControllers[pluginKey] = pluginControllerTypes;
        }

        public IEnumerable<IPluginController> GetPluginControllers(string pluginKey)
        {
            var controllers = _pluginControllers[pluginKey];

            var serviceProvider = _serviceProviders[pluginKey];

            return controllers.Select(controllerType =>
            {
                var services = controllerType.GetConstructors().First().GetParameters()
                    .Select(parameter => serviceProvider.GetService(parameter.ParameterType)).ToArray();

                return (IPluginController) Activator.CreateInstance(controllerType, services);
            });
        }

        public void SetServiceProvider(string pluginKey, AppDashPlugin pluginInstance)
        {
            var serviceCollection = new ServiceCollection();

            pluginInstance.ConfigureServices(serviceCollection);

            _serviceProviders[pluginKey] = serviceCollection.BuildServiceProvider();
        }

        public IServiceProvider GetServiceProvider(string pluginKey)
        {
            if (!_serviceProviders.ContainsKey(pluginKey))
                return null;

            return _serviceProviders[pluginKey];
        }
    }
}
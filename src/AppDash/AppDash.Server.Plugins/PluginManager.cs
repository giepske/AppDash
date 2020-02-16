using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AppDash.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Plugins
{
    public class PluginManager
    {
        private readonly PluginResolver _pluginResolver;

        private readonly IServiceCollection _serviceCollection;
        private IServiceProvider _serviceProvider;

        public PluginManager(PluginResolver pluginResolver)
        {
            _pluginResolver = pluginResolver;

            _serviceCollection = new ServiceCollection()
                .AddDependencies();
        }

        public void LoadPlugins(string pluginPath)
        {
            _pluginResolver.ClearPlugins();

            string[] plugins = Directory.GetFiles(pluginPath, "*.dll");

            foreach (string plugin in plugins)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFile(plugin);

                    var pluginTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(AppDashPlugin)).ToList();

                    foreach (Type pluginType in pluginTypes)
                    {
                        _serviceCollection.AddSingleton(typeof(AppDashPlugin), pluginType);
                    }

                    _pluginResolver.AddPlugins(pluginTypes);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            _serviceProvider = _serviceCollection.BuildServiceProvider();

            Console.WriteLine($"Loaded {_pluginResolver.GetPlugins().Count} plugin(s).");
        }

        public T GetPlugin<T>(string key)
        {
            Type pluginType = _pluginResolver.GetPlugin(key);

            return (T)_serviceProvider.GetService(pluginType);
        }

        public IEnumerable<AppDashPlugin> GetPlugins()
        {
            return _serviceProvider.GetServices<AppDashPlugin>();
        }

        public void InitializePlugins()
        {
            List<AppDashPlugin> pluginInstances = _serviceProvider.GetServices<AppDashPlugin>().ToList();

            foreach (KeyValuePair<string, Type> plugin in _pluginResolver.GetPlugins())
            {
                AppDashPlugin pluginInstance =
                    pluginInstances.First(pluginInstance1 => pluginInstance1.GetType() == plugin.Value);

                pluginInstance.Key = plugin.Key;
            }
        }
    }
}

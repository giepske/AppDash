using System.Collections.Generic;
using AppDash.Plugins;
using AppDash.Plugins.Controllers;
using AppDash.Plugins.Tiles;

namespace AppDash.Server.Plugins
{
    public class PluginManager
    {
        private readonly PluginResolver _pluginResolver;
        private readonly PluginLoader _pluginLoader;
        private readonly PluginInitializer _pluginInitializer;

        public PluginManager(PluginResolver pluginResolver, PluginLoader pluginLoader, PluginInitializer pluginInitializer)
        {
            _pluginResolver = pluginResolver;
            _pluginLoader = pluginLoader;
            _pluginInitializer = pluginInitializer;
        }

        public void LoadPlugins(string pluginPath)
        {
            _pluginLoader.LoadPlugins(pluginPath);

            _pluginInitializer.InitializePlugins();
        }

        public AppDashPlugin GetPluginInstance(string pluginKey)
        {
            return _pluginResolver.GetPluginInstance(pluginKey);
        }

        public IEnumerable<AppDashPlugin> GetPluginInstances()
        {
            return _pluginResolver.GetPluginInstances();
        }

        public IEnumerable<ITile> GetTiles()
        {
            return _pluginResolver.GetPluginTiles();
        }

        public IEnumerable<IPluginController> GetPluginControllers(string pluginKey)
        {
            return _pluginResolver.GetPluginControllers(pluginKey);
        }

        public IEnumerable<Plugin> GetPlugins()
        {
            return _pluginResolver.GetPlugins();
        }
    }
}

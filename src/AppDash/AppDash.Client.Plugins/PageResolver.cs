using System;
using System.Collections.Generic;
using System.Linq;
using AppDash.Plugins;
using AppDash.Plugins.Pages;

namespace AppDash.Client.Plugins
{
    public class PageResolver
    {
        private readonly PluginResolver _pluginResolver;

        //Pages listed using <RelativePath>, <PageComponent>
        private Dictionary<string, PluginPageComponent> _pages;
        //Plugins listed using <PageComponent>, <AppDashPlugin>
        private Dictionary<PluginPageComponent, AppDashPlugin> _plugins;

        public PageResolver(PluginResolver pluginResolver)
        {
            _pluginResolver = pluginResolver;
            _pages = new Dictionary<string, PluginPageComponent>();
            _plugins = new Dictionary<PluginPageComponent, AppDashPlugin>();
        }

        public PluginPageComponent AddPage(Type pageType)
        {
            var pageInstance = (PluginPageComponent) Activator.CreateInstance(pageType);

            _pages[pageInstance.RelativePath] = pageInstance;

            var pluginType = pageType.Assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.BaseType.GetGenericTypeDefinition() == typeof(PluginPage<,>) &&
                type.BaseType?.GenericTypeArguments[1] == pageType)
                .BaseType.GenericTypeArguments[0];

            var pluginInstance = _pluginResolver.GetPlugin(pluginType);

            _plugins[pageInstance] = pluginInstance;

            return pageInstance;
        }

        public Dictionary<string, PluginPageComponent> GetPages()
        {
            return _pages;
        }

        public PluginPageComponent GetPage(string key)
        {
            return _pages[key];
        }

        public void ClearPages()
        {
            _pages.Clear();
        }

        public void SetPage(PluginPageComponent component)
        {
            _pages[component.GetType().FullName] = component;
        }

        public PluginPageComponent GetMainPage(Type pluginType)
        {
            return _plugins.FirstOrDefault(plugin => 
                plugin.Value.GetType() == pluginType && plugin.Key.IsMainPage).Key;
        }
    }
}
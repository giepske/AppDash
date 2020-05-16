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
        private Dictionary<string, PageComponent> _pages;
        //Plugins listed using <PageComponent>, <AppDashPlugin>
        private Dictionary<PageComponent, AppDashPlugin> _plugins;

        public PageResolver(PluginResolver pluginResolver)
        {
            _pluginResolver = pluginResolver;
            _pages = new Dictionary<string, PageComponent>();
            _plugins = new Dictionary<PageComponent, AppDashPlugin>();
        }

        public PageComponent AddPage(Type pageType)
        {
            var pageInstance = (PageComponent) Activator.CreateInstance(pageType);

            _pages[pageInstance.RelativePath] = pageInstance;

            var pluginType = pageType.Assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.BaseType.GetGenericTypeDefinition() == typeof(Page<,>) &&
                type.BaseType?.GenericTypeArguments[1] == pageType)
                .BaseType.GenericTypeArguments[0];

            var pluginInstance = _pluginResolver.GetPlugin(pluginType);

            _plugins[pageInstance] = pluginInstance;

            return pageInstance;
        }

        public Dictionary<string, PageComponent> GetPages()
        {
            return _pages;
        }

        public PageComponent GetPage(string key)
        {
            return _pages[key];
        }

        public void ClearPages()
        {
            _pages.Clear();
        }

        public void SetPage(PageComponent component)
        {
            _pages[component.GetType().FullName] = component;
        }

        public PageComponent GetMainPage(Type pluginType)
        {
            return _plugins.FirstOrDefault(plugin => 
                plugin.Value.GetType() == pluginType && plugin.Key.IsMainPage).Key;
        }
    }
}
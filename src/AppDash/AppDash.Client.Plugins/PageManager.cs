using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Plugins;
using AppDash.Plugins.Pages;

namespace AppDash.Client.Plugins
{
    public class PageManager
    {
        private readonly PluginResolver _pluginResolver;

        public readonly SemaphoreSlim PageLock;

        public PageManager(PluginResolver pluginResolver)
        {
            _pluginResolver = pluginResolver;
            PageLock = new SemaphoreSlim(1, 1);
        }
        
        public IEnumerable<PluginPageComponent> LoadPages(Assembly assembly)
        {
            var pageTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(PluginPageComponent)).ToList();

            foreach (Type pageType in pageTypes)
            {
                yield return _pluginResolver.LoadPluginPageComponent(pageType);
            }
        }

        public void InitializePages()
        {
            //todo initialize page data
        }

        public async Task<IEnumerable<PluginPageComponent>> GetPages()
        {
            await PageLock.WaitAsync();

            var pages = _pluginResolver.GetPageComponents();

            PageLock.Release();

            return pages;
        }

        public async Task<PluginPageComponent> GetPage(string pluginKey, string pageKey)
        {
            await PageLock.WaitAsync();

            var page = _pluginResolver.GetPage(pluginKey, pageKey);

            PageLock.Release();

            return page;
        }

        public async Task SetPluginPageComponent(PluginPageComponent component)
        {
            await PageLock.WaitAsync();

            _pluginResolver.SetPluginPageComponent(component);

            PageLock.Release();
        }

        public async Task<PluginPageComponent> GetMainPage(Type pluginType)
        {
            await PageLock.WaitAsync();

            var mainPage = _pluginResolver.GetMainPage(pluginType);

            PageLock.Release();

            return mainPage;
        }
    }
}

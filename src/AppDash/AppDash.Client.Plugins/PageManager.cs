using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AppDash.Plugins.Pages;

namespace AppDash.Client.Plugins
{
    public class PageManager
    {
        private readonly PageResolver _pageResolver;

        public readonly SemaphoreSlim PageLock;

        public PageManager(PageResolver pageResolver)
        {
            _pageResolver = pageResolver;
            PageLock = new SemaphoreSlim(1, 1);
        }
        
        public IEnumerable<PageComponent> LoadPages(Assembly assembly)
        {
            var pageTypes = assembly.GetTypes().Where(type => type.BaseType == typeof(PageComponent)).ToList();

            foreach (Type pageType in pageTypes)
            {
                yield return _pageResolver.AddPage(pageType);
            }
        }

        public void InitializePages()
        {
            //todo initialize page data
        }

        public async Task<IEnumerable<PageComponent>> GetPages()
        {
            await PageLock.WaitAsync();

            var pages = _pageResolver.GetPages().Values;

            PageLock.Release();

            return pages;
        }

        public async Task<PageComponent> GetPage(string pageKey)
        {
            await PageLock.WaitAsync();

            var page = _pageResolver.GetPage(pageKey);

            PageLock.Release();

            return page;
        }

        public async Task SetPage(PageComponent component)
        {
            await PageLock.WaitAsync();

            _pageResolver.SetPage(component);

            PageLock.Release();
        }

        public async Task<PageComponent> GetMainPage(Type pluginType)
        {
            await PageLock.WaitAsync();

            var mainPage = _pageResolver.GetMainPage(pluginType);

            PageLock.Release();

            return mainPage;
        }
    }
}

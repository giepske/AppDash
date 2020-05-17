using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Plugins;
using AppDash.Plugins.Pages;
using AppDash.Plugins.Settings;
using AppDash.Plugins.Tiles;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace AppDash.Client.Plugins
{
    /// <summary>
    /// Downloads, loads and initializes plugins and related items.
    /// </summary>
    public class PluginLoader
    {
        private readonly HttpClient _httpClient;
        private readonly PluginManager _pluginManager;
        private readonly TileManager _tileManager;
        private readonly NavigationManager _navigationManager;
        private readonly PageManager _pageManager;
        private readonly SettingsManager _settingsManager;

        /// <summary>
        /// This can be called when <see cref="LoadPlugins"/> is called. Fired when a plugin gets downloaded/loaded.
        /// <para>
        /// <see langword="string"></see> will be the filename.
        /// </para>
        /// </summary>
        public Action<string> OnPluginLoadStart;

        /// <summary>
        /// This can be called when <see cref="LoadPlugins"/> is called. Fired when tiles gets loaded from an assembly.
        /// <para>
        /// <see cref="Assembly"/> will be the assembly that is being loaded.
        /// </para>
        /// </summary>
        public Action<Assembly> OnTilesLoadStart;

        /// <summary>
        /// This can be called when <see cref="LoadPlugins"/> is called. Fired when pages gets loaded from an assembly.
        /// <para>
        /// <see cref="Assembly"/> will be the assembly that is being loaded.
        /// </para>
        /// </summary>
        public Action<Assembly> OnPagesLoadStart;

        /// <summary>
        /// This can be called when <see cref="LoadPlugins"/> is called. Fired when settings gets loaded from an assembly.
        /// <para>
        /// <see cref="Assembly"/> will be the assembly that is being loaded.
        /// </para>
        /// </summary>
        public Action<Assembly> OnSettingsLoadStart;

        public PluginLoader(HttpClient httpClient, PluginManager pluginManager, TileManager tileManager, NavigationManager navigationManager, PageManager pageManager, SettingsManager settingsManager)
        {
            _httpClient = httpClient;
            _pluginManager = pluginManager;
            _tileManager = tileManager;
            _navigationManager = navigationManager;
            _pageManager = pageManager;
            _settingsManager = settingsManager;
        }

        /// <summary>
        /// Downloads, loads and initializes all plugins from the server, also loading and initializing related items.
        /// <para>
        /// Use OnPluginLoadStart,
        /// </para>
        /// </summary>
        /// <returns></returns>
        public async Task LoadPlugins()
        {
            await _pluginManager.PluginLock.WaitAsync();
            await _tileManager.TileLock.WaitAsync();
            await _pageManager.PageLock.WaitAsync();
            await _settingsManager.SettingsLock.WaitAsync();

            //download and load plugins
            try
            {
                ApiResult result =
                    await _httpClient.GetJson<ApiResult>(_navigationManager.BaseUri + "api/plugins");
                var plugins = result.GetData<JArray>();

                var assemblies = plugins.Select(plugin => plugin["assembly"]?["url"]?.Value<string>())
                    .Distinct();

                foreach (string assemblyUrl in assemblies)
                {
                    OnPluginLoadStart?.Invoke(Path.GetFileName(assemblyUrl));

                    try
                    {
                        var assembly = await _pluginManager.LoadAssembly(assemblyUrl);

                        Dictionary<string, string> pluginKeys = plugins.ToDictionary(
                            plugin => plugin["key"].Value<string>(),
                            plugin => plugin["name"].Value<string>());

                        _pluginManager.LoadPlugins(assembly, pluginKeys);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _pluginManager.PluginLock.Release();
            }

            await LoadTiles();
            await LoadPages();
            await LoadSettings();
        }

        private async Task LoadTiles()
        {
            //load tiles from plugins
            try
            {
                List<TileComponent> tiles = new List<TileComponent>();

                foreach (var assembly in await _pluginManager.GetAssemblies())
                {
                    OnTilesLoadStart?.Invoke(assembly);

                    try
                    {
                        tiles.AddRange(_tileManager.LoadTiles(assembly));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                foreach (TileComponent tileComponent in tiles)
                {
                    //TODO this doesnt do anything yet, just like the method below. FIX!
                }

                try
                {
                    _tileManager.InitializeTiles();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _tileManager.TileLock.Release();
            }
        }

        private async Task LoadPages()
        {
            //load pages from plugins
            try
            {
                List<PageComponent> pages = new List<PageComponent>();

                foreach (var assembly in await _pluginManager.GetAssemblies())
                {
                    OnPagesLoadStart?.Invoke(assembly);

                    try
                    {
                        pages.AddRange(_pageManager.LoadPages(assembly));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                foreach (PageComponent pageComponent in pages)
                {
                    //TODO this doesnt do anything yet, just like the method below. FIX!
                }

                try
                {
                    _pageManager.InitializePages();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _pageManager.PageLock.Release();
            }

            Console.WriteLine($"Loaded {(await _pageManager.GetPages()).Count()} pages");
        }

        private async Task LoadSettings()
        {
            //load settings from plugins
            try
            {
                List<SettingsComponent> settings = new List<SettingsComponent>();

                foreach (var assembly in await _pluginManager.GetAssemblies())
                {
                    OnSettingsLoadStart?.Invoke(assembly);

                    try
                    {
                        settings.AddRange(_settingsManager.LoadSettings(assembly));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                foreach (SettingsComponent settingsComponent in settings)
                {
                    //TODO this doesnt do anything yet, just like the method below. FIX!
                }

                try
                {
                    _settingsManager.InitializeSettings();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                _settingsManager.SettingsLock.Release();
            }

            Console.WriteLine($"Loaded {(await _settingsManager.GetSettings()).Count()} settings");
        }
    }
}

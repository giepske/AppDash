﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Plugins;
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

        public PluginLoader(HttpClient httpClient, PluginManager pluginManager, TileManager tileManager, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _pluginManager = pluginManager;
            _tileManager = tileManager;
            _navigationManager = navigationManager;
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

            //download and load plugins
            try
            {
                ApiResult result =
                    await _httpClient.GetJsonAsync<ApiResult>(_navigationManager.BaseUri + "api/plugins");
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
    }
}

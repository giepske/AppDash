using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using AppDash.Core;
using AppDash.Plugins.Controllers;
using AppDash.Plugins.Pages;
using AppDash.Plugins.Settings;
using AppDash.Plugins.Tiles;

namespace AppDash.Plugins
{
    /// <summary>
    /// A plugin that holds a <see cref="AppDashPlugin"/> instance and relevant types and instances of
    /// <see cref="PluginTile{TPlugin,TRazorComponent}"/>,
    /// <see cref="PluginSettings{TPlugin,TRazorComponent}"/> and
    /// <see cref="PluginPage{TPlugin,TRazorComponent}"/>.
    /// </summary>
    public class Plugin
    {
        public string PluginKey { get; set; }
        public string CssFileUrl { get; set; }
        public string JsFileUrl { get; set; }
        public Type PluginType { get; set; }
        public AppDashPlugin PluginInstance { get; set; }
        public PluginSettingsComponent PluginSettingsComponent { get; set; }
        public ISettings PluginSettings { get; set; }
        public Dictionary<string, Tuple<ITile, PluginTileComponent>> PluginTiles;
        public Dictionary<string, Tuple<IPage, PluginPageComponent>> PluginPages;
        public List<IPluginController> PluginControllers;

        public Plugin(Type pluginType, string pluginKey)
        {
            bool cssFileExists = File.Exists(Path.Combine(Config.ApplicationDirectory, "plugins", 
                pluginType.Assembly.GetName().Name, "css", pluginType.Name + ".css"));

            bool jsFileExists = File.Exists(Path.Combine(Config.ApplicationDirectory, "plugins",
                pluginType.Assembly.GetName().Name, "js", pluginType.Name + ".js"));

            PluginKey = pluginKey;
            CssFileUrl = cssFileExists ? $"/plugins/{pluginType.Assembly.GetName().Name}/css/{pluginType.Name}.css" : null;
            JsFileUrl = jsFileExists ? $"/plugins/{pluginType.Assembly.GetName().Name}/js/{pluginType.Name}.js" : null;
            PluginType = pluginType;
            PluginInstance = (AppDashPlugin)Activator.CreateInstance(pluginType);
            PluginInstance.Key = pluginKey;
            PluginTiles = new Dictionary<string, Tuple<ITile, PluginTileComponent>>();
            PluginPages = new Dictionary<string, Tuple<IPage, PluginPageComponent>>();
            PluginControllers = new List<IPluginController>();
        }
    }

    public class PluginResolver
    {
        private Dictionary<string, Plugin> _plugins;

        public PluginResolver()
        {
            _plugins = new Dictionary<string, Plugin>();
        }

        public Plugin AddPlugin(Type pluginType, string pluginKey)
        {
            var pluginInstance = new Plugin(pluginType, pluginKey);

            _plugins[pluginKey] = pluginInstance;

            return pluginInstance;
        }

        public IEnumerable<AppDashPlugin> GetPluginInstances()
        {
            return _plugins.Values.Select(plugin => plugin.PluginInstance);
        }

        public Plugin GetPlugin(string pluginKey)
        {
            return _plugins.Values.FirstOrDefault(plugin => plugin.PluginKey == pluginKey);
        }

        public Plugin GetPlugin(Type pluginType)
        {
            return _plugins.Values.FirstOrDefault(plugin => plugin.PluginType == pluginType);
        }

        public void ClearPlugins()
        {
            _plugins.Clear();
        }

        public AppDashPlugin GetPluginInstance(Type pluginType)
        {
            return _plugins.Values.FirstOrDefault(plugin => plugin.PluginType == pluginType)?.PluginInstance;
        }

        public AppDashPlugin GetPluginInstance(string pluginKey)
        {
            return _plugins.Values.FirstOrDefault(plugin => plugin.PluginKey == pluginKey)?.PluginInstance;
        }

        public PluginSettingsComponent LoadPluginSettingsComponent(Type settingsComponentType, HttpClient httpClient)
        {
            Console.WriteLine("LoadPluginSettings");

            var settingsComponentInstance = CreateInstance<PluginSettingsComponent>(settingsComponentType);

            var pluginSettingsType = settingsComponentType.Assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.GetGenericTypeDefinition() == typeof(PluginSettings<,>) &&
                type.BaseType?.GenericTypeArguments[1] == settingsComponentType);

            if (pluginSettingsType == null)
                return null;

            var plugin = GetPlugin(pluginSettingsType.BaseType?.GenericTypeArguments[0]);

            plugin.PluginSettingsComponent = settingsComponentInstance;
            //plugin.PluginSettings = CreateInstance<ISettings>(pluginSettingsType, plugin.PluginInstance);

            settingsComponentInstance.HttpClient = httpClient;
            settingsComponentInstance.PluginKey = plugin.PluginKey;

            return settingsComponentInstance;
        }

        public void LoadPluginSettings(Type pluginType)
        {
            var pluginSettingsType = pluginType.Assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.GetGenericTypeDefinition() == typeof(PluginSettings<,>) &&
                type.BaseType?.GenericTypeArguments[0] == pluginType);

            if (pluginSettingsType == null)
                return;

            var plugin = GetPlugin(pluginSettingsType.BaseType?.GenericTypeArguments[0]);

            plugin.PluginSettings = CreateInstance<ISettings>(pluginSettingsType, plugin.PluginInstance);
        }

        public PluginSettingsComponent SetPluginSettingsComponent(PluginSettingsComponent pluginSettingsComponent)
        {
            Console.WriteLine("SetPluginSettingsComponent");

            Plugin plugin;

            if (string.IsNullOrEmpty(pluginSettingsComponent.PluginKey))
            {
                plugin = _plugins.FirstOrDefault(plugin1 => 
                    plugin1.Value.PluginSettingsComponent?.GetType() == pluginSettingsComponent.GetType()).Value;
            }
            else
            {
                plugin = GetPlugin(pluginSettingsComponent.PluginKey);
            }

            pluginSettingsComponent.SettingsData = plugin.PluginSettingsComponent.SettingsData;
            pluginSettingsComponent.HttpClient = plugin.PluginSettingsComponent.HttpClient;
            pluginSettingsComponent.PluginKey = plugin.PluginSettingsComponent.PluginKey;

            plugin.PluginSettingsComponent = pluginSettingsComponent;

            return pluginSettingsComponent;
        }

        public IEnumerable<PluginSettingsComponent> GetSettingComponents()
        {
            return _plugins.Select(plugin => plugin.Value.PluginSettingsComponent)
                .Where(pluginSettingsComponent => pluginSettingsComponent != null);
        }

        private T CreateInstance<T>(Type type, params object[] args)
        {
            if(args.Length == 0)
                return (T)Activator.CreateInstance(type);

            return (T)Activator.CreateInstance(type, args);
        }

        public PluginSettingsComponent GetSettingComponent(string pluginKey)
        {
            var plugin = GetPlugin(pluginKey);

            return plugin.PluginSettingsComponent;
        }

        public void SetPluginTileComponent(string tileKey, PluginTileComponent pluginTileComponent)
        {
            Console.WriteLine("SetPluginTileComponent");

            Plugin plugin = _plugins.FirstOrDefault(plugin1 =>
                plugin1.Value.PluginTiles.ContainsKey(tileKey)).Value;

            var oldPluginTile = plugin.PluginTiles[tileKey];

            pluginTileComponent.Data = oldPluginTile.Item2.Data;
            pluginTileComponent.PluginKey = oldPluginTile.Item2.PluginKey;
            pluginTileComponent.TileKey = oldPluginTile.Item2.TileKey;
            pluginTileComponent.PluginSettings = oldPluginTile.Item2.PluginSettings;

            plugin.PluginTiles[tileKey] = new Tuple<ITile, PluginTileComponent>(oldPluginTile.Item1, pluginTileComponent);
        }

        public PluginTileComponent LoadPluginTileComponent(Type tileComponentType, string tileKey)
        {
            Console.WriteLine("LoadPluginTile");

            var tileComponentInstance = CreateInstance<PluginTileComponent>(tileComponentType);

            var pluginTileType = tileComponentType.Assembly.GetTypes().FirstOrDefault(type => 
                (type.BaseType?.BaseType?.IsGenericType ?? false) && 
                type.BaseType?.BaseType?.GetGenericTypeDefinition() == typeof(PluginTile<,>) &&
                type.BaseType?.GenericTypeArguments[1] == tileComponentType);

            var plugin = GetPlugin(pluginTileType.BaseType.GenericTypeArguments[0]);

            plugin.PluginTiles.Add(tileKey, new Tuple<ITile, PluginTileComponent>(null, tileComponentInstance));

            Console.WriteLine("LoadPluginTile sdfdsf " + tileKey);
            tileComponentInstance.PluginKey = plugin.PluginKey;
            tileComponentInstance.TileKey = tileKey;

            return tileComponentInstance;
        }

        public IEnumerable<PluginTileComponent> GetPluginTileComponents()
        {
            return _plugins.SelectMany(plugin => plugin.Value.PluginTiles.Values.Select(tile => tile.Item2))
                .Where(pluginTileComponent => pluginTileComponent != null);
        }

        public PluginTileComponent GetPluginTileComponent(string pluginKey, string tileKey)
        {
            var plugin = _plugins[pluginKey];

            return plugin.PluginTiles[tileKey].Item2;
        }

        public PluginPageComponent LoadPluginPageComponent(Type pageComponentType)
        {
            Console.WriteLine("LoadPluginPage");

            var pageComponentInstance = CreateInstance<PluginPageComponent>(pageComponentType);

            var pluginPageType = pageComponentType.Assembly.GetTypes().FirstOrDefault(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.BaseType?.GetGenericTypeDefinition() == typeof(PluginPage<,>) &&
                type.BaseType?.GenericTypeArguments[1] == pageComponentType);

            var plugin = GetPlugin(pluginPageType.BaseType.GenericTypeArguments[0]);

            plugin.PluginPages.Add(pageComponentInstance.RelativePath, 
                new Tuple<IPage, PluginPageComponent>(null, pageComponentInstance));

            Console.WriteLine("LoadPluginPage sdfdsf " + plugin.PluginKey);
            pageComponentInstance.PluginKey = plugin.PluginKey;

            return pageComponentInstance;
        }

        public IEnumerable<PluginPageComponent> GetPageComponents()
        {
            return _plugins.SelectMany(plugin => plugin.Value.PluginPages.Values.Select(page => page.Item2))
                .Where(pluginPageComponent => pluginPageComponent != null);
        }

        public PluginPageComponent GetPage(string pluginKey, string pageKey)
        {
            var plugin = GetPlugin(pluginKey);

            return plugin.PluginPages.Values.FirstOrDefault(page => page.Item2.RelativePath == pageKey)?.Item2;
        }

        public PluginPageComponent GetMainPage(Type pluginType)
        {
            var plugin = GetPlugin(pluginType);

            return plugin.PluginPages.Values.FirstOrDefault(page => page.Item2.IsMainPage)?.Item2;
        }

        public void SetPluginPageComponent(PluginPageComponent pluginPageComponent)
        {
            Console.WriteLine("SetPluginPageComponent");

            Plugin plugin = _plugins.FirstOrDefault(plugin1 =>
                plugin1.Value.PluginPages.ContainsKey(pluginPageComponent.RelativePath)).Value;

            var oldPluginPage = plugin.PluginPages[pluginPageComponent.RelativePath];

            pluginPageComponent.Data = oldPluginPage.Item2.Data;
            pluginPageComponent.PluginKey = oldPluginPage.Item2.PluginKey;
            pluginPageComponent.PluginSettings = oldPluginPage.Item2.PluginSettings;

            plugin.PluginPages[pluginPageComponent.RelativePath] = new Tuple<IPage, PluginPageComponent>(oldPluginPage.Item1, pluginPageComponent);
        }

        public ISettings GetSettings(string pluginKey)
        {
            var plugin = GetPlugin(pluginKey);

            return plugin.PluginSettings;
        }

        public IEnumerable<ITile> GetPluginTiles(string pluginKey)
        {
            var plugin = GetPlugin(pluginKey);

            return plugin.PluginTiles.Values.Select(tile => tile.Item1);
        }

        public IEnumerable<ITile> GetPluginTiles()
        {
            return _plugins.SelectMany(plugin => plugin.Value.PluginTiles.Values.Select(tile => tile.Item1));
        }

        public IEnumerable<IPage> GetPluginPages(string pluginKey)
        {
            var plugin = GetPlugin(pluginKey);

            return plugin.PluginPages.Values.Select(page => page.Item1);
        }

        public void LoadPluginControllers(Type pluginType)
        {
            var plugin = GetPlugin(pluginType);

            var controllerTypes = pluginType.Assembly.GetTypes().Where(type =>
                (type.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.GetGenericTypeDefinition() == typeof(PluginController<>) &&
                type.BaseType?.GenericTypeArguments[0] == pluginType);

            foreach (Type controllerType in controllerTypes)
            {
                plugin.PluginControllers.Add(CreateInstance<IPluginController>(controllerType));
            }
        }

        public IEnumerable<IPluginController> GetPluginControllers(string pluginKey)
        {
            var plugin = GetPlugin(pluginKey);

            return plugin?.PluginControllers;
        }

        public void LoadPluginTile(Type pluginType, Type tileType, string tileKey)
        {
            var plugin = GetPlugin(pluginType);

            var tile = new Tuple<ITile, PluginTileComponent>(CreateInstance<ITile>(tileType, plugin.PluginInstance), null);

            tile.Item1.PluginKey = plugin.PluginKey;
            tile.Item1.TileKey = tileKey;

            plugin.PluginTiles.Add(tileKey, tile);
        }

        public void LoadPluginTile(Type pluginType, ITile tileInstance, string tileKey)
        {
            var plugin = GetPlugin(pluginType);

            var tile = new Tuple<ITile, PluginTileComponent>(tileInstance, null);

            tile.Item1.PluginKey = plugin.PluginKey;
            tile.Item1.TileKey = tileKey;

            plugin.PluginTiles.Add(tileKey, tile);
        }

        public void LoadPluginPages(Type pluginType)
        {
            var pluginPageTypes = pluginType.Assembly.GetTypes().Where(type =>
                (type.BaseType?.BaseType?.IsGenericType ?? false) &&
                type.BaseType?.BaseType?.GetGenericTypeDefinition() == typeof(PluginPage<,>) &&
                type.BaseType?.GenericTypeArguments[0] == pluginType);

            var plugin = GetPlugin(pluginType);

            foreach (Type pluginPageType in pluginPageTypes)
            {
                var pageComponentInstance = CreateInstance<PluginPageComponent>(pluginPageType.BaseType.GenericTypeArguments[1]);

                plugin.PluginPages.Add(pageComponentInstance.RelativePath, 
                    new Tuple<IPage, PluginPageComponent>(CreateInstance<IPage>(pluginPageType, plugin.PluginInstance), null));
            }
        }

        public IEnumerable<Plugin> GetPlugins()
        {
            return _plugins.Values;
        }
    }
}
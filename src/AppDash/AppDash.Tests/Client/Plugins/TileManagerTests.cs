using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AppDash.Client.Plugins;
using AppDash.Plugins;
using AppDash.Tests.TestPlugin;
using Moq;
using Xunit;

namespace AppDash.Tests.Client.Plugins
{
    public class TileManagerTests
    {
        /// <summary>
        /// Test if the LoadTiles loads the tiles correctly.
        /// </summary>
        [Fact]
        public void LoadTilesTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            TileManager tileManager = new TileManager(pluginResolver, pluginManager, null);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            //Act
            tileManager.LoadTiles(Assembly.GetExecutingAssembly(), new List<Tuple<string, Type>>
            {
                new Tuple<string, Type>("TestTileKey", typeof(TestTileComponent))
            }).ToList();

            //Assert
            var plugin = pluginResolver.GetPlugin("TestKey");

            Assert.Single(plugin.PluginTiles);
            Assert.Equal("TestTileKey", plugin.PluginTiles.First().Key);
            Assert.Null(plugin.PluginTiles.First().Value.Item1);
            Assert.NotNull(plugin.PluginTiles.First().Value.Item2);
        }

        /// <summary>
        /// Test if the GetPluginTileComponent returns the right component.
        /// </summary>
        [Fact]
        public async Task GetPluginTileComponentTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            TileManager tileManager = new TileManager(pluginResolver, pluginManager, null);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            tileManager.LoadTiles(Assembly.GetExecutingAssembly(), new List<Tuple<string, Type>>
            {
                new Tuple<string, Type>("TestTileKey", typeof(TestTileComponent))
            }).ToList();

            //Act
            var component = await tileManager.GetPluginTileComponent("TestKey", "TestTileKey");

            //Assert
            Assert.NotNull(component);
            Assert.Equal(typeof(TestTileComponent).FullName, component.GetType().FullName);
        }

        /// <summary>
        /// Test if the SetPluginTileComponent sets the right component.
        /// </summary>
        [Fact]
        public async Task SetPluginTileComponentTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            TileManager tileManager = new TileManager(pluginResolver, pluginManager, null);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            tileManager.LoadTiles(Assembly.GetExecutingAssembly(), new List<Tuple<string, Type>>
            {
                new Tuple<string, Type>("TestTileKey", typeof(TestTileComponent))
            }).ToList();

            var oldComponent = await tileManager.GetPluginTileComponent("TestKey", "TestTileKey");

            var data = new PluginData();
            var pluginSettings = new PluginData();

            oldComponent.Data = data;
            oldComponent.PluginSettings = pluginSettings;

            //Act
            await tileManager.SetPluginTileComponent("TestTileKey", new TestTileComponent
            {
                Data = data,
                PluginSettings = pluginSettings
            });

            var newComponent = await tileManager.GetPluginTileComponent("TestKey", "TestTileKey");

            //Assert
            Assert.NotNull(newComponent);
            Assert.Equal("TestKey", newComponent.PluginKey);
            Assert.Equal("TestTileKey", newComponent.TileKey);
            Assert.Equal(oldComponent.Data, newComponent.Data);
            Assert.Equal(oldComponent.PluginSettings, newComponent.PluginSettings);
        }
    }
}
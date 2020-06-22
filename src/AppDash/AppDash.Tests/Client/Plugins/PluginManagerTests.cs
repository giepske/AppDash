using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AppDash.Client.Plugins;
using AppDash.Plugins;
using AppDash.Tests.TestPlugin;
using Xunit;

namespace AppDash.Tests.Client.Plugins
{
    public class PluginManagerTests
    {
        /// <summary>
        /// Test if the LoadPlugins loads the plugin correctly.
        /// </summary>
        [Fact]
        public void LoadPluginsTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);

            //Act
            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));
            
            //Assert
            //we expect 1 plugin
            Assert.Single(pluginResolver.GetPlugins());
            Assert.Equal(typeof(TestPlugin.TestPlugin).FullName, pluginResolver.GetPlugins().First().PluginInstance.GetType().FullName);
        }

        /// <summary>
        /// Test if the GetPluginInstance returns the right plugin.
        /// </summary>
        [Fact]
        public void GetPluginInstanceTest()
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
            });

            //Act
            var plugin = pluginManager.GetPluginInstance(new TestTileComponent());

            //Assert
            //we expect 1 plugin
            Assert.Equal(typeof(TestPlugin.TestPlugin).FullName, plugin.GetType().FullName);
            Assert.Equal("TestKey", plugin.Key);
        }

        /// <summary>
        /// Test if the GetPluginInstances returns the right plugins.
        /// </summary>
        [Fact]
        public async Task GetPluginInstancesTest()
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
            });

            //Act
            var plugins = await pluginManager.GetPluginInstances();

            //Assert
            //we expect 1 plugin
            Assert.Single(plugins);
        }
    }
}

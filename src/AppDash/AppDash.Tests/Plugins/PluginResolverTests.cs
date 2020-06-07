using System.Linq;
using AppDash.Plugins;
using AppDash.Tests.TestPlugin;
using Xunit;

namespace AppDash.Tests.Plugins
{
    [Collection("Sequential")]
    public class PluginResolverTests
    {
        /// <summary>
        /// Test if AddPlugin adds the plugin with the right data.
        /// </summary>
        [Fact]
        public void AddPluginTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            //Act
            var plugin = pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Assert

            //make sure the plugin and PluginInstance is not null
            Assert.NotNull(plugin);
            Assert.NotNull(plugin.PluginInstance);
            //make sure the plugin has the right key
            Assert.Equal("TestKey", plugin.PluginInstance.Key);
            Assert.Equal("TestKey", plugin.PluginKey);

            Assert.Empty(plugin.PluginControllers);
            Assert.Empty(plugin.PluginPages);
            Assert.Empty(plugin.PluginTiles);
            Assert.Null(plugin.PluginSettings);
            Assert.Null(plugin.PluginSettingsComponent);
            Assert.Equal(typeof(TestPlugin.TestPlugin), plugin.PluginType);
        }

        /// <summary>
        /// Test if GetPlugin returns the right plugin.
        /// </summary>
        [Fact]
        public void GetPluginTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Act
            var plugin = pluginResolver.GetPlugin(typeof(TestPlugin.TestPlugin));
            var pluginAgain = pluginResolver.GetPlugin("TestKey");

            //Assert
            Assert.Equal(plugin, pluginAgain);

            //make sure the plugin and PluginInstance is not null
            Assert.NotNull(plugin);
            Assert.NotNull(plugin.PluginInstance);
            //make sure the plugin has the right key
            Assert.Equal("TestKey", plugin.PluginInstance.Key);
            Assert.Equal("TestKey", pluginAgain.PluginInstance.Key);
            Assert.Equal("TestKey", plugin.PluginKey);
            Assert.Equal("TestKey", pluginAgain.PluginKey);
        }

        /// <summary>
        /// Test if LoadPluginTile loads the right plugin tile.
        /// </summary>
        [Fact]
        public void LoadPluginTileTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Act
            pluginResolver.LoadPluginTile(typeof(TestPlugin.TestPlugin), typeof(TestTile), "TestTileKey");

            //Assert
            var plugin = pluginResolver.GetPlugin(typeof(TestPlugin.TestPlugin));

            Assert.Single(plugin.PluginTiles);
            Assert.True(plugin.PluginTiles.ContainsKey("TestTileKey"));

            //make sure the tile instance is not null
            Assert.NotNull(plugin.PluginTiles["TestTileKey"].Item1);
            //we only load the ITile so the component SHOULD be null
            Assert.Null(plugin.PluginTiles["TestTileKey"].Item2);
            Assert.Equal("TestTileKey", plugin.PluginTiles["TestTileKey"].Item1.TileKey);
            Assert.Equal("TestKey", plugin.PluginTiles["TestTileKey"].Item1.PluginKey);
        }

        /// <summary>
        /// Test if LoadPluginTileComponent loads the right plugin tile component.
        /// </summary>
        [Fact]
        public void LoadPluginTileComponentTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Act
            pluginResolver.LoadPluginTileComponent(typeof(TestTileComponent), "TestTileKey");

            //Assert
            var plugin = pluginResolver.GetPlugin(typeof(TestPlugin.TestPlugin));

            Assert.Single(plugin.PluginTiles);
            Assert.True(plugin.PluginTiles.ContainsKey("TestTileKey"));

            //make sure the tile component is not null
            Assert.NotNull(plugin.PluginTiles["TestTileKey"].Item2);
            //we only load the component so the ITile SHOULD be null
            Assert.Null(plugin.PluginTiles["TestTileKey"].Item1);
            Assert.Equal("TestTileKey", plugin.PluginTiles["TestTileKey"].Item2.TileKey);
            Assert.Equal("TestKey", plugin.PluginTiles["TestTileKey"].Item2.PluginKey);
        }

        /// <summary>
        /// Test if GetPluginTiles returns the right plugin tiles.
        /// </summary>
        [Fact]
        public void GetPluginTilesTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginTile(typeof(TestPlugin.TestPlugin), typeof(TestTile), "TestTileKey");
            pluginResolver.LoadPluginTile(typeof(TestPlugin.TestPlugin), typeof(TestTile2), "TestTile2Key");

            //Act
            var tiles = pluginResolver.GetPluginTiles("TestKey").ToList();

            //Assert
            Assert.Equal(2, tiles.Count);
            Assert.Equal(1, tiles.Count(tile => tile.TileKey == "TestTileKey"));
            Assert.Equal(1, tiles.Count(tile => tile.TileKey == "TestTile2Key"));

            //make sure the tiles are not null
            Assert.NotNull(tiles.First(tile => tile.TileKey == "TestTileKey"));
            Assert.NotNull(tiles.First(tile => tile.TileKey == "TestTileKey"));
        }

        /// <summary>
        /// Test if GetPluginTileComponent returns the right plugin tile component.
        /// </summary>
        [Fact]
        public void GetPluginTileComponentTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginTileComponent(typeof(TestTileComponent), "TestTileKey");

            //Act
            var tileComponent = pluginResolver.GetPluginTileComponent("TestKey", "TestTileKey");

            //Assert
            Assert.NotNull(tileComponent);
            Assert.Equal(typeof(TestTileComponent), tileComponent.GetType());
            Assert.Equal("TestTileKey", tileComponent.TileKey);
            Assert.Equal("TestKey", tileComponent.PluginKey);
        }

        /// <summary>
        /// Test if SetPluginTileComponent sets the right plugin tile component.
        /// </summary>
        [Fact]
        public void SetPluginTileComponentTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginTileComponent(typeof(TestTileComponent), "TestTileKey");

            var oldComponent = pluginResolver.GetPluginTileComponent("TestKey", "TestTileKey");
            oldComponent.Data = new PluginData();
            oldComponent.PluginSettings = new PluginData();

            var newComponent = new TestTileComponent();

            //Act
            pluginResolver.SetPluginTileComponent("TestTileKey", newComponent);

            //Assert
            Assert.NotEqual(oldComponent, pluginResolver.GetPluginTileComponent("TestKey", "TestTileKey"));
            Assert.Equal(newComponent,pluginResolver.GetPluginTileComponent("TestKey", "TestTileKey"));
            Assert.Equal("TestKey", newComponent.PluginKey);
            Assert.Equal("TestTileKey", newComponent.TileKey);
            Assert.NotNull(newComponent.Data);
            Assert.NotNull(newComponent.PluginSettings);
        }

        /// <summary>
        /// Test if LoadPluginPages loads the right plugin pages.
        /// </summary>
        [Fact]
        public void LoadPluginPagesTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Act
            pluginResolver.LoadPluginPages(typeof(TestPlugin.TestPlugin));

            //Assert
            var plugin = pluginResolver.GetPlugin(typeof(TestPlugin.TestPlugin));

            Assert.Equal(2, plugin.PluginPages.Count);

            //make sure the page instance is not null
            Assert.NotNull(plugin.PluginPages.First().Value.Item1);
            //we only load the IPage so the component SHOULD be null
            Assert.Null(plugin.PluginPages.First().Value.Item2);
            Assert.Equal("TestKey", plugin.PluginPages.First().Value.Item1.PluginKey);
        }

        /// <summary>
        /// Test if LoadPluginPageComponent loads the right plugin page component.
        /// </summary>
        [Fact]
        public void LoadPluginPageComponentTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Act
            pluginResolver.LoadPluginPageComponent(typeof(TestPageComponent));

            //Assert
            var plugin = pluginResolver.GetPlugin(typeof(TestPlugin.TestPlugin));

            Assert.Single(plugin.PluginPages);
            Assert.True(plugin.PluginPages.ContainsKey("/test"));

            //make sure the page component is not null
            Assert.NotNull(plugin.PluginPages["/test"].Item2);
            //we only load the component so the IPage SHOULD be null
            Assert.Null(plugin.PluginPages["/test"].Item1);
            Assert.Equal("TestKey", plugin.PluginPages["/test"].Item2.PluginKey);
        }

        /// <summary>
        /// Test if GetPluginPages returns the right plugin pages.
        /// </summary>
        [Fact]
        public void GetPluginPagesTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginPages(typeof(TestPlugin.TestPlugin));

            //Act
            var pages = pluginResolver.GetPluginPages("TestKey").ToList();

            //Assert
            Assert.Equal(2, pages.Count);
            Assert.Equal(2, pages.Count(page => page.PluginKey == "TestKey"));

            //make sure the right amount of pages are in the list
            Assert.Single(pages.Where(tile => tile.GetType() == typeof(TestPage)));
            Assert.Single(pages.Where(tile => tile.GetType() == typeof(MainTestPage)));
        }

        /// <summary>
        /// Test if GetPageComponents returns the right plugin page components.
        /// </summary>
        [Fact]
        public void GetPageComponentsTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginPageComponent(typeof(TestPageComponent));
            pluginResolver.LoadPluginPageComponent(typeof(MainTestPageComponent));

            //Act
            var pageComponents = pluginResolver.GetPageComponents().ToList();

            //Assert
            Assert.NotNull(pageComponents);
            Assert.Equal(2, pageComponents.Count);
            Assert.Equal("TestKey", pageComponents[0].PluginKey);
            Assert.Equal("TestKey", pageComponents[1].PluginKey);
            Assert.True(pageComponents[1].IsMainPage);
        }

        /// <summary>
        /// Test if GetMainPage returns the right plugin page.
        /// </summary>
        [Fact]
        public void GetMainPageTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginPageComponent(typeof(TestPageComponent));
            pluginResolver.LoadPluginPageComponent(typeof(MainTestPageComponent));

            //Act
            var mainPage = pluginResolver.GetMainPage(typeof(TestPlugin.TestPlugin));

            //Assert
            Assert.NotNull(mainPage);
            Assert.Equal(typeof(MainTestPageComponent), mainPage.GetType());
        }

        /// <summary>
        /// Test if SetPluginPageComponent sets the right plugin page component.
        /// </summary>
        [Fact]
        public void SetPluginPageComponentTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginPageComponent(typeof(TestPageComponent));

            var oldComponent = pluginResolver.GetPageComponents().First();
            oldComponent.Data = new PluginData();
            oldComponent.PluginSettings = new PluginData();

            var newComponent = new TestPageComponent();

            //Act
            pluginResolver.SetPluginPageComponent(newComponent);

            //Assert
            Assert.NotEqual(oldComponent, pluginResolver.GetPageComponents().First());
            Assert.Equal("TestKey", newComponent.PluginKey);
            Assert.NotNull(newComponent.Data);
            Assert.NotNull(newComponent.PluginSettings);
        }

        /// <summary>
        /// Test if LoadPluginSettings loads the right plugin settings.
        /// </summary>
        [Fact]
        public void LoadPluginSettingsTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Act
            pluginResolver.LoadPluginSettings(typeof(TestPlugin.TestPlugin));

            //Assert
            var plugin = pluginResolver.GetPlugin(typeof(TestPlugin.TestPlugin));

            Assert.NotNull(plugin.PluginSettings);
        }

        /// <summary>
        /// Test if LoadPluginSettingsComponent loads the right plugin settings component.
        /// </summary>
        [Fact]
        public void LoadPluginSettingsComponentTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");

            //Act
            pluginResolver.LoadPluginSettingsComponent(typeof(TestSettingsComponent), null);

            //Assert
            var plugin = pluginResolver.GetPlugin(typeof(TestPlugin.TestPlugin));

            Assert.NotNull(plugin.PluginSettingsComponent);
            Assert.Equal(typeof(TestSettingsComponent), plugin.PluginSettingsComponent.GetType());
        }

        /// <summary>
        /// Test if GetSettings returns the right plugin settings.
        /// </summary>
        [Fact]
        public void GetSettingsTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginSettings(typeof(TestPlugin.TestPlugin));

            //Act
            var settings = pluginResolver.GetSettings("TestKey");

            //Assert
            Assert.NotNull(settings);
        }

        /// <summary>
        /// Test if GetSettingComponents returns the right plugin settings components.
        /// </summary>
        [Fact]
        public void GetSettingComponentsTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginSettingsComponent(typeof(TestSettingsComponent), null);

            //Act
            var settingsComponents = pluginResolver.GetSettingComponents().ToList();

            //Assert
            Assert.NotNull(settingsComponents);
            Assert.Single(settingsComponents);
        }
        
        /// <summary>
        /// Test if SetPluginSettingsComponent sets the right plugin setting component.
        /// </summary>
        [Fact]
        public void SetPluginSettingsComponentTest()
        {
            //Arrange
            PluginResolver pluginResolver = new PluginResolver();

            pluginResolver.AddPlugin(typeof(TestPlugin.TestPlugin), "TestKey");
            pluginResolver.LoadPluginSettingsComponent(typeof(TestSettingsComponent), null);

            var oldComponent = pluginResolver.GetSettingComponent("TestKey");

            var newComponent = new TestSettingsComponent();

            //Act
            pluginResolver.SetPluginSettingsComponent(newComponent);

            //Assert
            Assert.NotEqual(oldComponent, pluginResolver.GetSettingComponent("TestKey"));
            Assert.Equal("TestKey", newComponent.PluginKey);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using AppDash.Server.Core.Domain.Tiles;
using AppDash.Server.Data;
using AppDash.Server.Plugins;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Plugin = AppDash.Server.Core.Domain.Plugins.Plugin;

namespace AppDash.Tests.Server.Plugins
{
    public class PluginInitializerTests
    {
        private readonly Mock<Repository<Plugin>> _pluginRepositoryMock;
        private readonly Mock<Repository<Tile>> _tileRepositoryMock;
        private readonly Mock<Repository<PluginSettings>> _pluginSettingsRepositoryMock;

        public PluginInitializerTests()
        {
            _pluginRepositoryMock = new Mock<Repository<Plugin>>();
            _tileRepositoryMock = new Mock<Repository<Tile>>();
            _pluginSettingsRepositoryMock = new Mock<Repository<PluginSettings>>();
        }


        /// <summary>
        /// Test if the TestPlugin gets initialized correctly.
        /// </summary>
        [Fact]
        public void InitializePluginsTest()
        {
            //Arrange
            AppDashTestContext dbContext = new AppDashTestContext();

            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IRepository<Plugin>, Repository<Plugin>>(e => _pluginRepositoryMock.Object);
            serviceCollection.AddSingleton<IRepository<Tile>, Repository<Tile>>(e => _tileRepositoryMock.Object);
            serviceCollection.AddSingleton<IRepository<PluginSettings>, Repository<PluginSettings>>(e => _pluginSettingsRepositoryMock.Object);
            serviceCollection.AddSingleton<DbContext, AppDashTestContext>(e => dbContext);

            _pluginRepositoryMock.SetupRepositoryMock(dbContext, options =>
            {
                options.InsertNoTracking(new Plugin
                {
                    UniqueIdentifier = typeof(TestPlugin.TestPlugin).FullName,
                    Key = "TestKey",
                    Name = "TestName"
                });
            });

            _tileRepositoryMock.SetupRepositoryMock(dbContext, options =>
            { });

            _pluginSettingsRepositoryMock.SetupRepositoryMock(dbContext, options =>
            {
                options.InsertNoTracking(new PluginSettings
                {
                    PluginKey = "TestKey",
                    Data = JsonConvert.SerializeObject(new PluginData
                    {
                        Data = new Dictionary<string, Tuple<Type, object>>(new List<KeyValuePair<string, Tuple<Type, object>>>
                        {
                            new KeyValuePair<string, Tuple<Type, object>>("TestData", new Tuple<Type, object>(typeof(string), "TestValue")),
                            new KeyValuePair<string, Tuple<Type, object>>("TestData2", new Tuple<Type, object>(typeof(string), "TestValue2"))
                        })
                    })
                });
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var pluginResolver = new PluginResolver();
            var pluginSettingsManager = new PluginSettingsManager(serviceProvider);

            PluginLoader pluginLoader = new PluginLoader(pluginResolver, serviceProvider);

            PluginInitializer pluginInitializer = new PluginInitializer(pluginResolver, pluginSettingsManager, serviceProvider);

            //Act
            pluginLoader.LoadPlugins(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins"));

            pluginInitializer.InitializePlugins();

            //Assert
            var plugin = pluginResolver.GetPlugin("TestKey");

            //make sure the plugin exists
            Assert.NotNull(plugin);
            //make sure the plugin has the right key
            Assert.Equal("TestKey", plugin.PluginInstance.Key);
            Assert.Equal("TestKey", plugin.PluginKey);

            foreach (ITile pluginTile in pluginResolver.GetPluginTiles())
            {
                //make sure tiles have correct data
                Assert.Equal("TestKey", pluginTile.PluginKey);
                Assert.NotEmpty(pluginTile.TileKey);
                Assert.NotNull(pluginTile.PluginSettings);
                Assert.NotNull(pluginTile.PluginSettings.GetData<string>("TestData"));
                Assert.NotNull(pluginTile.PluginSettings.GetData<string>("TestData2"));
                Assert.Throws<InvalidCastException>(() =>
                {
                    //make sure this throws an InvalidCastException since it should be a string
                    int data = pluginTile.PluginSettings.GetData<int>("TestData");
                });
            }
        }
    }
}

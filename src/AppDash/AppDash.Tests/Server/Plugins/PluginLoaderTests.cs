using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Tiles;
using AppDash.Server.Data;
using AppDash.Server.Plugins;
using AppDash.Tests.TestPlugin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Plugin = AppDash.Server.Core.Domain.Plugins.Plugin;

namespace AppDash.Tests.Server.Plugins
{
    public class PluginLoaderTests
    {
        private readonly Mock<Repository<Plugin>> _pluginRepositoryMock;
        private readonly Mock<Repository<Tile>> _tileRepositoryMock;

        public PluginLoaderTests()
        {
            _pluginRepositoryMock = new Mock<Repository<Plugin>>();
            _tileRepositoryMock = new Mock<Repository<Tile>>();
        }


        /// <summary>
        /// Test if the TestPlugin gets loaded correctly.
        /// </summary>
        [Fact]
        public void LoadPluginsTest()
        {
            //Arrange
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IRepository<Plugin>, Repository<Plugin>>(e => _pluginRepositoryMock.Object);
            serviceCollection.AddScoped<IRepository<Tile>, Repository<Tile>>(e => _tileRepositoryMock.Object);
            serviceCollection.AddScoped<DbContext, AppDashTestContext>();

            _pluginRepositoryMock.SetupRepositoryMock(options =>
            { });

            _tileRepositoryMock.SetupRepositoryMock(options =>
            { });

            var pluginResolver = new PluginResolver();

            PluginLoader pluginLoader = new PluginLoader(pluginResolver, serviceCollection.BuildServiceProvider());

            //Act
            pluginLoader.LoadPlugins(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins"));
            
            //Assert
            //we expect 1 plugin
            Assert.Single(pluginResolver.GetPlugins());
            //the plugin has 5 tiles
            Assert.Equal(5, pluginResolver.GetPluginTiles().Count());
            //4 of them are type TestTile2
            Assert.Equal(4, pluginResolver.GetPluginTiles().Count(tile => tile.GetType().FullName == typeof(TestTile2).FullName));

            foreach (ITile pluginTile in pluginResolver.GetPluginTiles())
            {
                Assert.NotEmpty(pluginTile.TileKey);
                Assert.NotEmpty(pluginTile.PluginKey);
            }
        }
    }
}

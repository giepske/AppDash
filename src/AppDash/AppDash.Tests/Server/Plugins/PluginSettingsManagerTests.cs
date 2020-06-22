using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AppDash.Plugins;
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
    public class PluginSettingsManagerTests
    {
        private readonly Mock<Repository<Plugin>> _pluginRepositoryMock;
        private readonly Mock<Repository<Tile>> _tileRepositoryMock;
        private readonly Mock<Repository<PluginSettings>> _pluginSettingsRepositoryMock;

        public PluginSettingsManagerTests()
        {
            _pluginRepositoryMock = new Mock<Repository<Plugin>>();
            _tileRepositoryMock = new Mock<Repository<Tile>>();
            _pluginSettingsRepositoryMock = new Mock<Repository<PluginSettings>>();
        }

        /// <summary>
        /// Test if GetPluginSettings returns the right plugin settings.
        /// </summary>
        [Theory]
        [InlineData("WrongPluginKey", false)]
        [InlineData("TestKey", true)]
        public void GetPluginSettingsTest(string pluginKey, bool shouldHaveData)
        {
            //Arrange
            AppDashTestContext dbContext = new AppDashTestContext();
            dbContext.Database.EnsureDeleted();

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

            PluginLoader pluginLoader = new PluginLoader(pluginResolver, serviceProvider);

            PluginSettingsManager pluginSettingsManager = new PluginSettingsManager(serviceProvider);

            var pluginInitializer = new PluginInitializer(pluginResolver, pluginSettingsManager, serviceProvider);

            //Act
            pluginLoader.LoadPlugins(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins"));

            pluginInitializer.InitializePlugins();

            var pluginData = pluginSettingsManager.GetPluginSettings(pluginKey);

            //Assert
            if (shouldHaveData)
            {
                Assert.NotNull(pluginData);

                var testData = pluginData.GetData<string>("TestData");
                var testData2 = pluginData.GetData<string>("TestData2");

                Assert.NotEmpty(testData);
                Assert.NotEmpty(testData2);
                Assert.Equal(2, pluginData.Data.Count);

                var defaultData = pluginData.GetData<string>("InvalidData");

                Assert.Equal(default, defaultData);
            }
            else
            {
                Assert.Null(pluginData);
            }
        }

        /// <summary>
        /// Test if GetPluginSettings returns the right plugin settings.
        /// </summary>
        [Theory]
        [InlineData("WrongPluginKey", false)]
        [InlineData("TestKey", true)]
        public void SetPluginSettingsTest(string pluginKey, bool shouldHaveData)
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

            PluginLoader pluginLoader = new PluginLoader(pluginResolver, serviceProvider);

            PluginSettingsManager pluginSettingsManager = new PluginSettingsManager(serviceProvider);

            var pluginInitializer = new PluginInitializer(pluginResolver, pluginSettingsManager, serviceProvider);

            var pluginData = new PluginData
            {
                Data = new Dictionary<string, Tuple<Type, object>>(new List<KeyValuePair<string, Tuple<Type, object>>>
                {
                    new KeyValuePair<string, Tuple<Type, object>>("TestData", new Tuple<Type, object>(typeof(string), "test"))
                })
            };

            //Act
            pluginLoader.LoadPlugins(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins"));

            pluginInitializer.InitializePlugins();

            bool result = pluginSettingsManager.SetPluginSettings(pluginKey, pluginData);
            
            //Assert
            if (shouldHaveData)
            {
                Assert.True(result);

                Assert.NotNull(pluginData);

                var testData = pluginData.GetData<string>("TestData");
                var testData2 = pluginData.GetData<string>("TestData2");

                Assert.NotEmpty(testData);
                Assert.Equal("test", testData);
                Assert.Equal(default, testData2);
                Assert.Single(pluginData.Data);
            }
            else
            {
                Assert.False(result);
            }
        }
    }
}

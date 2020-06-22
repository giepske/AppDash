using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AppDash.Plugins;
using AppDash.Plugins.Controllers;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using AppDash.Server.Core.Domain.Tiles;
using AppDash.Server.Data;
using AppDash.Server.Plugins;
using AppDash.Tests.TestPlugin.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using Xunit;
using Plugin = AppDash.Server.Core.Domain.Plugins.Plugin;

namespace AppDash.Tests.Server.Plugins
{
    public class PluginControllerMatcherTests
    {
        private readonly Mock<Repository<Plugin>> _pluginRepositoryMock;
        private readonly Mock<Repository<Tile>> _tileRepositoryMock;
        private readonly Mock<Repository<PluginSettings>> _pluginSettingsRepositoryMock;

        public PluginControllerMatcherTests()
        {
            _pluginRepositoryMock = new Mock<Repository<Plugin>>();
            _tileRepositoryMock = new Mock<Repository<Tile>>();
            _pluginSettingsRepositoryMock = new Mock<Repository<PluginSettings>>();
        }

        /// <summary>
        /// Test if TryMatch matches the correct route.
        /// </summary>
        [Theory]
        [InlineData("WrongPluginKey", "/", false, null, null)]
        [InlineData("TestKey", "/", false, null, null)]
        [InlineData("TestKey", "/test/WorkingRoute", true, typeof(TestController2), "WorkingRoute")]
        [InlineData("TestKey", "/test/WorkingRoute2", true, typeof(TestController2), "WorkingRoute2")]
        [InlineData("TestKey", "test/WorkingRoute", true, typeof(TestController2), "WorkingRoute")]
        [InlineData("TestKey", "test/WorkingRoute2", true, typeof(TestController2), "WorkingRoute2")]
        [InlineData("TestKey", "WorkingRoute2", false, null, null)]
        [InlineData("TestKey", "/WorkingRoute", true, typeof(TestController), "WorkingRoute")]
        [InlineData("TestKey", "/workingroute", true, typeof(TestController), "WorkingRoute")]
        [InlineData("TestKey", "/adsafwe/WorkingRoute", false, null, null)]
        [InlineData("TestKey", "/adsafwe/WorkingRoute/wefwe", false, null, null)]
        [InlineData("TestKey", "WorkingRoute", true, typeof(TestController), "WorkingRoute")]
        [InlineData("TestKey", "workingroute", true, typeof(TestController), "WorkingRoute")]
        [InlineData("TestKey", "/sfdfregdf", false, null, null)]
        [InlineData("TestKey", "/sfdfregdf/", false, null, null)]
        [InlineData("TestKey", "", false, null, null)]
        [InlineData("TestKey", null, false, null, null)]
        [InlineData(null, "test", false, null, null)]
        [InlineData(null, null, false, null, null)]
        public void TryMatchTest(string pluginKey, string route, bool expectedResult, Type expectedController, string expectedMethodName)
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

            var pluginSettingsManager = new PluginSettingsManager(serviceProvider);

            PluginLoader pluginLoader = new PluginLoader(pluginResolver, serviceProvider);

            var pluginInitializer = new PluginInitializer(pluginResolver, pluginSettingsManager, serviceProvider);

            var pluginManager = new PluginManager(pluginResolver, pluginLoader, pluginInitializer);

            PluginControllerMatcher pluginControllerMatcher = new PluginControllerMatcher(pluginManager);

            //Act
            pluginLoader.LoadPlugins(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins"));

            pluginInitializer.InitializePlugins();

            bool result = pluginControllerMatcher.TryMatch(pluginKey, route, out (IPluginController, MethodInfo) controller);

            //Assert
            Assert.Equal(expectedResult, result);

            if (expectedResult)
            {
                Assert.NotNull(controller.Item1);
                Assert.NotNull(controller.Item2);
                Assert.Equal(expectedController.FullName, controller.Item1.GetType().FullName);
                Assert.Equal(expectedMethodName, controller.Item2.Name);
            }
            else
            {
                Assert.Null(controller.Item1);
                Assert.Null(controller.Item2);
            }
        }

        /// <summary>
        /// Test if Execute matches the correct result.
        /// </summary>
        [Theory]
        [InlineData("TestKey", "/test/WorkingRoute", true)]
        [InlineData("TestKey", "/test/WorkingRoute2", "test")]
        [InlineData("TestKey", "test/WorkingRoute", true)]
        [InlineData("TestKey", "test/WorkingRoute2", "test")]
        [InlineData("TestKey", "/WorkingRoute", false)]
        [InlineData("TestKey", "/workingroute", false)]
        [InlineData("TestKey", "WorkingRoute", false)]
        [InlineData("TestKey", "workingroute", false)]
        public void ExecuteTest(string pluginKey, string route, object expectedResult)
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

            var pluginSettingsManager = new PluginSettingsManager(serviceProvider);

            PluginLoader pluginLoader = new PluginLoader(pluginResolver, serviceProvider);

            var pluginInitializer = new PluginInitializer(pluginResolver, pluginSettingsManager, serviceProvider);

            var pluginManager = new PluginManager(pluginResolver, pluginLoader, pluginInitializer);

            PluginControllerMatcher pluginControllerMatcher = new PluginControllerMatcher(pluginManager);

            //Act
            pluginLoader.LoadPlugins(Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "plugins"));

            pluginInitializer.InitializePlugins();

            bool result = pluginControllerMatcher.TryMatch(pluginKey, route, out (IPluginController, MethodInfo) controller);

            //the test controllers return a ObjectResult, we cast it so we can easily check the value below
            var actionResult = (ObjectResult)pluginControllerMatcher.Execute(pluginKey, route, null, controller.Item1, controller.Item2);

            //Assert
            Assert.True(result);
            Assert.Equal(expectedResult, actionResult.Value);
        }
    }
}

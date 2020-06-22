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
    public class PageManagerTests
    {
        /// <summary>
        /// Test if the LoadPages loads the pages correctly.
        /// </summary>
        [Fact]
        public void LoadPagesTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            PageManager pageManager = new PageManager(pluginResolver);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            //Act
            pageManager.LoadPages(Assembly.GetExecutingAssembly()).ToList();

            //Assert
            var plugin = pluginResolver.GetPlugin("TestKey");

            Assert.Equal(2, plugin.PluginPages.Count);
            Assert.Equal("TestKey", plugin.PluginPages.First().Value.Item2.PluginKey);
        }

        /// <summary>
        /// Test if the GetPage returns the right component.
        /// </summary>
        [Fact]
        public async Task GetPageTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            PageManager pageManager = new PageManager(pluginResolver);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            pageManager.LoadPages(Assembly.GetExecutingAssembly()).ToList();

            //Act
            var component = await pageManager.GetPage("TestKey", "/test");

            //Assert
            Assert.NotNull(component);
            Assert.Equal(typeof(TestPageComponent).FullName, component.GetType().FullName);
        }

        /// <summary>
        /// Test if the GetMainPage returns the right component.
        /// </summary>
        [Fact]
        public async Task GetMainPageTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            PageManager pageManager = new PageManager(pluginResolver);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            pageManager.LoadPages(Assembly.GetExecutingAssembly()).ToList();

            //Act
            var component = await pageManager.GetMainPage(typeof(TestPlugin.TestPlugin));

            //Assert
            Assert.NotNull(component);
            Assert.True(component.IsMainPage);
            Assert.Equal(typeof(MainTestPageComponent).FullName, component.GetType().FullName);
        }

        /// <summary>
        /// Test if the SetPluginTileComponent sets the right component.
        /// </summary>
        [Fact]
        public async Task SetPluginPageComponentTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            PageManager pageManager = new PageManager(pluginResolver);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            pageManager.LoadPages(Assembly.GetExecutingAssembly()).ToList();

            var oldComponent = await pageManager.GetPage("TestKey", "/test");

            var data = new PluginData();
            var pluginSettings = new PluginData();

            oldComponent.Data = data;
            oldComponent.PluginSettings = pluginSettings;

            //Act
            await pageManager.SetPluginPageComponent(new TestPageComponent
            {
                Data = data,
                PluginSettings = pluginSettings
            });

            var newComponent = await pageManager.GetPage("TestKey", "/test");

            //Assert
            Assert.NotNull(newComponent);
            Assert.Equal("TestKey", newComponent.PluginKey);
            Assert.Equal("/test", newComponent.RelativePath);
            Assert.Equal(oldComponent.Data, newComponent.Data);
            Assert.Equal(oldComponent.PluginSettings, newComponent.PluginSettings);
        }
    }
}
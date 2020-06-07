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
    public class SettingsManagerTests
    {
        /// <summary>
        /// Test if the LoadSettings loads the settings correctly.
        /// </summary>
        [Fact]
        public void LoadSettingsTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            SettingsManager settingsManager = new SettingsManager(pluginManager, null, pluginResolver, null);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            //Act
            settingsManager.LoadSettings(Assembly.GetExecutingAssembly()).ToList();

            //Assert
            var plugin = pluginResolver.GetPlugin("TestKey");

            Assert.NotNull(plugin.PluginSettingsComponent);
            Assert.Equal("TestKey", plugin.PluginSettingsComponent.PluginKey);
        }

        /// <summary>
        /// Test if the GetSettingComponent returns the right component.
        /// </summary>
        [Fact]
        public async Task GetSettingComponentTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            SettingsManager settingsManager = new SettingsManager(pluginManager, null, pluginResolver, null);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            settingsManager.LoadSettings(Assembly.GetExecutingAssembly()).ToList();

            //Act
            var component = await settingsManager.GetSettingComponent("TestKey");

            //Assert
            Assert.NotNull(component);
            Assert.Equal(typeof(TestSettingsComponent).FullName, component.GetType().FullName);
        }

        /// <summary>
        /// Test if the SetPluginSettingComponent sets the right component.
        /// </summary>
        [Fact]
        public async Task SetPluginSettingComponentTest()
        {
            //Arrange
            var pluginResolver = new PluginResolver();

            PluginManager pluginManager = new PluginManager(pluginResolver);
            SettingsManager settingsManager = new SettingsManager(pluginManager, null, pluginResolver, null);

            pluginManager.LoadPlugins(Assembly.GetExecutingAssembly(), new Dictionary<string, string>(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("TestKey", nameof(TestPlugin.TestPlugin))
            }));

            settingsManager.LoadSettings(Assembly.GetExecutingAssembly()).ToList();

            var oldComponent = await settingsManager.GetSettingComponent("TestKey");

            var pluginSettings = new PluginData();

            oldComponent.SettingsData = pluginSettings;

            //Act
            await settingsManager.SetPluginSettingsComponent(new TestSettingsComponent
            {
                SettingsData = pluginSettings
            });

            var newComponent = await settingsManager.GetSettingComponent("TestKey");

            //Assert
            Assert.NotNull(newComponent);
            Assert.Equal("TestKey", newComponent.PluginKey);
            Assert.Equal(oldComponent.SettingsData, newComponent.SettingsData);
        }
    }
}
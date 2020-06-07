using AppDash.Plugins.Settings;

namespace AppDash.Tests.TestPlugin
{
    public class TestSettings : PluginSettings<TestPlugin, TestSettingsComponent>
    {
        public TestSettings(TestPlugin plugin) : base(plugin)
        {
        }
    }
}
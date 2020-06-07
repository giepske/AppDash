using AppDash.Plugins.Pages;

namespace AppDash.Tests.TestPlugin
{
    public class TestPageComponent : PluginPageComponent
    {
        public override string RelativePath { get; set; } = "/test";
        public override bool IsMainPage { get; set; }

        public override bool OnUpdate()
        {
            return true;
        }
    }
}
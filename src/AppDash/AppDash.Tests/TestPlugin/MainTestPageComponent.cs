using AppDash.Plugins.Pages;

namespace AppDash.Tests.TestPlugin
{
    public class MainTestPageComponent : PluginPageComponent
    {
        public override string RelativePath { get; set; } = "/main";
        public override bool IsMainPage { get; set; } = true;

        public override bool OnUpdate()
        {
            return true;
        }
    }
}
using AppDash.Plugins.Controllers;

namespace AppDash.Tests.TestPlugin.Controllers
{
    public class TestController2 : PluginController<TestPlugin>
    {
        [PluginControllerRoute("/test/WorkingRoute")]
        public bool WorkingRoute()
        {
            return true;
        }

        [PluginControllerRoute("/test/WorkingRoute2")]
        public string WorkingRoute2()
        {
            return "test";
        }
    }
}
using AppDash.Plugins.Controllers;

namespace AppDash.Tests.TestPlugin.Controllers
{
    public class TestController : PluginController<TestPlugin>
    {
        [PluginControllerRoute("WorkingRoute")]
        public bool WorkingRoute()
        {
            return false;
        }
    }
}
using AppDash.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Tests.TestPlugin
{
    public class TestPlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "Test Plugin";

        public override string Description { get; set; } =
            "A plugin for unit testing, this plugin should not be used in production environment";

        public override string Icon { get; set; } = "icon.png";

        public override void ConfigureServices(IServiceCollection services)
        {
            // this doesn't work yet so we leave it empty.
        }
    }
}

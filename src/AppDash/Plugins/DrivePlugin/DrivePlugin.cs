using AppDash.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace DrivePlugin
{
    public class DrivePlugin : AppDashPlugin
    {
        public override string Name { get; set; } = "Drive Plugin";

        public override string Description { get; set; } =
            "A plugin that shows your drives and their free/total space.";
        public override string Icon { get; set; }

        public override void ConfigureServices(IServiceCollection services)
        {
            
        }
    }
}
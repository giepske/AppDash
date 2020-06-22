using AppDash.Plugins.Tiles;

namespace AppDash.Tests.TestPlugin
{
    public class TestTileComponent : PluginTileComponent
    {
        public override string Url { get; set; }

        public override bool OnUpdate()
        {
            return true;
        }
    }
}
using AppDash.Plugins.Tiles;

namespace AppDash.Tests.TestPlugin
{
    public class TestTile : RealtimePluginTile<TestPlugin, TestTileComponent>
    {
        public TestTile(TestPlugin plugin) : base(plugin)
        {
        }

        public override void Dispose()
        { }
    }
}
using AppDash.Plugins.Tiles;

namespace AppDash.Tests.TestPlugin
{
    public class TestTile2 : RealtimePluginTile<TestPlugin, TestTileComponent>
    {
        public TestTile2(TestPlugin plugin) : base(plugin)
        {
        }

        public override void Dispose()
        { }
    }
}
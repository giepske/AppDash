using System.Collections.Generic;
using AppDash.Plugins.Tiles;

namespace AppDash.Tests.TestPlugin
{
    public class TestTileFactory : ITileFactory<TestPlugin>
    {
        public IEnumerable<ITile> GetTiles(TestPlugin plugin)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new TestTile2(plugin);
            }
        }
    }
}
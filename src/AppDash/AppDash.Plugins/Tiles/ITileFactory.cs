using System.Collections.Generic;

namespace AppDash.Plugins.Tiles
{
    public interface ITileFactory<in TPlugin> where TPlugin : IPlugin
    {
        IEnumerable<ITile> GetTiles(TPlugin plugin);
    }
}
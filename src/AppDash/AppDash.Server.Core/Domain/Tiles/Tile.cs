using AppDash.Server.Core.Data;

namespace AppDash.Server.Core.Domain.Tiles
{
    public class Tile : BaseEntity
    {
        public string UniqueIdentifier { get; set; }
        public string Key { get; set; }
        public string TileFactory { get; set; }
        public int FactoryIndex { get; set; }
    }
}

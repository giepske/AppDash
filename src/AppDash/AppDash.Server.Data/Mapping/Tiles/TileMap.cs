using AppDash.Server.Core.Domain.Tiles;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDash.Server.Data.Mapping.Tiles
{
    public class TileMap : EntityMappingConfiguration<Tile>
    {
        public override void Configure(EntityTypeBuilder<Tile> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Key)
                .IsRequired();
            builder.Property(e => e.UniqueIdentifier)
                .IsRequired();
            builder.Property(e => e.TileFactory);
            builder.Property(e => e.FactoryIndex);
        }
    }
}
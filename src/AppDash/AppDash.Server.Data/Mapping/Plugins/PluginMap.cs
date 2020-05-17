using AppDash.Server.Core.Domain.Plugins;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDash.Server.Data.Mapping.Plugins
{
    public class PluginMap : EntityMappingConfiguration<Plugin>
    {
        public override void Configure(EntityTypeBuilder<Plugin> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Key)
                .IsRequired();
            builder.Property(e => e.Name)
                .IsRequired();
            builder.Property(e => e.UniqueIdentifier)
                .IsRequired();
            builder.Property(e => e.Enabled);
        }
    }
}
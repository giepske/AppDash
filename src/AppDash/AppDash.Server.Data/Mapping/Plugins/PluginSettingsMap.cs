using AppDash.Server.Core.Domain.Plugins;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDash.Server.Data.Mapping.Plugins
{
    public class PluginSettingsMap : EntityMappingConfiguration<PluginSettings>
    {
        public override void Configure(EntityTypeBuilder<PluginSettings> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.PluginKey)
                .IsRequired();
            builder.Property(e => e.Data)
                .IsRequired();
        }
    }
}

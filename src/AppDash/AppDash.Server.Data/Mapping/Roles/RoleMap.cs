using AppDash.Server.Core.Domain.Roles;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppDash.Server.Data.Mapping.Roles
{
    public class RoleMap : EntityMappingConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Permissions).IsRequired();
            builder.Property(e => e.CanBeModified).IsRequired();
            builder.Property(e => e.Color).IsRequired();
            builder.Property(e => e.Index).IsRequired();
        }
    }
}

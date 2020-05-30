using Microsoft.EntityFrameworkCore;

namespace AppDash.Server.Data.Mapping
{
    public interface IEntityMappingConfiguration
    {
        void ApplyConfiguration(ModelBuilder modelBuilder);
    }
}

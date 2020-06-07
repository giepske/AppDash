using AppDash.Server.Core.Data;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return serviceCollection;
        }
    }
}
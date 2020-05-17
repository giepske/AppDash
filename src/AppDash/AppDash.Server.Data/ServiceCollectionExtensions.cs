using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace AppDash.Server.Data
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
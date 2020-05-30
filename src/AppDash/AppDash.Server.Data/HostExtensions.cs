using AppDash.Core.Domain.Roles;
using AppDash.Server.Core.Domain.Roles;
using AppDash.Server.Core.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppDash.Server.Data
{
    public static class HostExtensions
    {
        /// <summary>
        /// Ensure that the database exists with all its tables and default data.
        /// </summary>
        /// <param name="host"></param>
        public static IHost EnsureDatabaseExists(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<AppDashContext>();

                var relationalDatabaseCreator =
                    (RelationalDatabaseCreator) dbContext.Database.GetService<IDatabaseCreator>();

                var databaseExists = relationalDatabaseCreator.Exists() && relationalDatabaseCreator.HasTables();
                
                dbContext.Database.Migrate();

                if (!databaseExists)
                {
                    RoleManager<Role> roleManager = scope.ServiceProvider.GetService<RoleManager<Role>>();
                    roleManager.CreateAsync(new Role("Administrator", Permissions.ALL, "#b30202", false)).Wait();

                    UserManager<User> userManager = scope.ServiceProvider.GetService<UserManager<User>>();
                    userManager.CreateAsync(new User("admin"), "admin").Wait();

                    var admin = userManager.FindByNameAsync("admin").Result;
                    userManager.AddToRoleAsync(admin, "Administrator").Wait();
                }
            }

            return host;
        }
    }
}
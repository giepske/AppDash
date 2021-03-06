﻿using System;
using System.Linq;
using System.Reflection;
using AppDash.Server.Core.Domain.Roles;
using AppDash.Server.Core.Domain.Users;
using AppDash.Server.Data.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AppDash.Server.Data
{
    public class AppDashTestContext : IdentityDbContext<User, Role, int>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("LightTownTests");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var configurations = Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                (t.BaseType?.IsGenericType ?? false) && t.BaseType?.GetGenericTypeDefinition() == typeof(EntityMappingConfiguration<>));

            foreach (Type configurationType in configurations)
            {
                var configuration = (IEntityMappingConfiguration)Activator.CreateInstance(configurationType);
                configuration?.ApplyConfiguration(modelBuilder);
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using AppDash.Core;
using AppDash.Core.Domain.Roles;
using AppDash.Plugins;
using AppDash.Plugins.Tiles;
using AppDash.Server.Core.Data;
using AppDash.Server.Core.Domain.Tiles;
using AppDash.Server.Data;
using AppDash.Server.Plugins;
using AppDash.Tests.TestPlugin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Plugin = AppDash.Server.Core.Domain.Plugins.Plugin;

namespace AppDash.Tests.Core
{
    public class PermissionMemoryCacheTests
    {
        /// <summary>
        /// Test if GetClients gets the right clients.
        /// </summary>
        [Theory]
        [InlineData(null, new []{"client1", "client2", "client3", "client4"})]
        [InlineData(Permissions.ALL, new []{"client3"})]
        [InlineData(Permissions.CREATE_PROJECTS, new []{"client3", "client4"})]
        [InlineData(Permissions.CREATE_PROJECTS | Permissions.MANAGE_PROJECTS, new []{"client3"})]
        public void GetClientsTest(Permissions? permissions, string[] expectedClients)
        {
            //Arrange
            PermissionMemoryCache permissionMemoryCache = new PermissionMemoryCache();

            permissionMemoryCache.AddClient("client1", null);
            permissionMemoryCache.AddClient("client2", null);
            permissionMemoryCache.AddClient("client3", Permissions.ALL);
            permissionMemoryCache.AddClient("client4", Permissions.CREATE_PROJECTS | Permissions.DELETE_PROJECTS);

            //Act
            var clients = permissionMemoryCache.GetClients(permissions);

            //Assert
            //we expect these clients
            Assert.Equal(expectedClients, clients);
        }

        /// <summary>
        /// Test if RemoveClient removes the right client.
        /// </summary>
        [Theory]
        [InlineData("client1")]
        [InlineData("client2")]
        [InlineData("client3")]
        [InlineData("client4")]
        public void RemoveClientTest(string client)
        {
            //Arrange
            PermissionMemoryCache permissionMemoryCache = new PermissionMemoryCache();

            permissionMemoryCache.AddClient("client1", null);
            permissionMemoryCache.AddClient("client2", null);
            permissionMemoryCache.AddClient("client3", Permissions.ALL);
            permissionMemoryCache.AddClient("client4", Permissions.CREATE_PROJECTS | Permissions.DELETE_PROJECTS);

            //Act
            permissionMemoryCache.RemoveClient(client);

            //get all clients
            var clients = permissionMemoryCache.GetClients(null).ToList();

            //Assert
            //we expect these clients
            Assert.DoesNotContain(client, clients);
            Assert.Equal(3, clients.Count);
        }
    }
}

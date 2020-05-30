using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Core.Domain.Roles;
using AppDash.Server.Core.Domain.Roles;
using AppDash.Server.Core.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace AppDash.Server.Core.Communication
{
    public class ChatHub : Hub
    {
        private readonly PermissionMemoryCache _permissionMemoryCache;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public ChatHub(PermissionMemoryCache permissionMemoryCache, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _permissionMemoryCache = permissionMemoryCache;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public override async Task OnConnectedAsync()
        {
            Permissions? totalPermissions = null;

            User user = await _userManager.GetUserAsync(Context.User);

            if (user != null)
            {
                IEnumerable<Role> roles = (await _userManager.GetRolesAsync(user))
                    .Select(e => _roleManager.FindByNameAsync(e).Result);

                IEnumerable<Permissions> permissions = roles.Select(e => e.Permissions);
                totalPermissions = Permissions.NONE;

                foreach (Permissions permission in permissions)
                {
                    if (!totalPermissions.Value.HasFlag(permission))
                        totalPermissions |= permission;
                }
            }

            _permissionMemoryCache.AddClient(Context.ConnectionId, totalPermissions);

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _permissionMemoryCache.RemoveClient(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

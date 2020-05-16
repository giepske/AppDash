using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDash.Core;
using AppDash.Core.Domain.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace AppDash.Plugins.Communication
{
    //public class ChatHub : Hub
    //{
    //    private readonly PermissionMemoryCache _permissionMemoryCache;
    //    private readonly UserManager<User> _userManager;
    //    private readonly RoleManager<Role> _roleManager;

    //    public ChatHub(PermissionMemoryCache permissionMemoryCache, UserManager<User> userManager, RoleManager<Role> roleManager)
    //    {
    //        _permissionMemoryCache = permissionMemoryCache;
    //        _userManager = userManager;
    //        _roleManager = roleManager;
    //    }

    //    public override async Task OnConnectedAsync()
    //    {
    //        var user = await _userManager.GetUserAsync(Context.User);
    //        IEnumerable<Role> roles = (await _userManager.GetRolesAsync(user))
    //            .Select(e => _roleManager.FindByNameAsync(e).Result);

    //        var permissions = roles.Select(e => e.Permissions);
    //        Permissions totalPermissions = Permissions.NONE;

    //        foreach (Permissions permission in permissions)
    //        {
    //            if (!totalPermissions.HasFlag(permission))
    //                totalPermissions |= permission;
    //        }

    //        _permissionMemoryCache.AddClient(Context.ConnectionId, totalPermissions);

    //        await base.OnConnectedAsync();
    //    }

    //    public override Task OnDisconnectedAsync(Exception exception)
    //    {
    //        _permissionMemoryCache.RemoveClient(Context.ConnectionId);

    //        return base.OnDisconnectedAsync(exception);
    //    }

    //    public async Task SendMessage(string user, string message)
    //    {
    //        await Clients.All.SendAsync("ReceiveMessage", user, message);
    //    }
    //}
}

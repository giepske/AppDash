using AppDash.Core.Domain.Roles;
using Microsoft.AspNetCore.Identity;

namespace AppDash.Server.Core.Domain.Roles
{
    public class Role : IdentityRole<int>
    {
        public bool CanBeModified { get; set; }
        public Permissions Permissions { get; set; }
        public string Color { get; set; }
        public int Index { get; set; }

        public Role(string name, Permissions permissions, bool canBeModified = true)
        {
            CanBeModified = canBeModified;
            base.Name = name;
            Permissions = permissions;
        }

        public Role() { }
    }
}
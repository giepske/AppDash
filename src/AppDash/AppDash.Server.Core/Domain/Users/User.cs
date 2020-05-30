using Microsoft.AspNetCore.Identity;

namespace AppDash.Server.Core.Domain.Users
{
    public class User : IdentityUser<int>
    {
        public UserStatus UserStatus { get; set; }

        public User(string username)
        {
            base.UserName = username;
        }

        public User() { }
    }
}
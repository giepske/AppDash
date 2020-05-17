using System;
using AppDash.Server.Core.Data;

namespace AppDash.Server.Core.Domain.Plugins
{
    public class Plugin : BaseEntity
    {
        public string UniqueIdentifier { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        //public List<PluginRolePermission> PluginRolePermissions { get; set; }
    }
}

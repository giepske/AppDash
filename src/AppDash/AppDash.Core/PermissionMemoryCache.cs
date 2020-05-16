using System.Collections.Generic;
using System.Linq;
using AppDash.Core.Domain.Roles;

namespace AppDash.Core
{
    /// <summary>
    /// A memory cache that holds client ids together with their permissions.
    /// <para>
    /// This class should be used when you want to do lots of permission checks fast within memory.
    /// </para>
    /// </summary>
    public class PermissionMemoryCache
    {
        private object _lockObj;
        private Dictionary<string, Permissions?> _clients;

        public PermissionMemoryCache()
        {
            _lockObj = new object();
            _clients = new Dictionary<string, Permissions?>();
        }

        public void AddClient(string clientId, Permissions? permissions)
        {
            lock (_lockObj)
            {
                _clients.Add(clientId, permissions);
            }
        }

        public void RemoveClient(string clientId)
        {
            lock (_lockObj)
            {
                _clients.Remove(clientId);
            }
        }

        public IEnumerable<string> GetClients(Permissions? permissions)
        {
            lock (_lockObj)
            {
                //if the permissions parameter is null we return all clients
                if (!permissions.HasValue)
                    return _clients.Select(client => client.Key);

                var clients = _clients
                    .Where(client => client.Value?.HasFlag(permissions) ?? false)
                    .Select(client => client.Key);

                return clients;
            }
        }
    }
}

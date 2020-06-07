using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AppDash.Client.Plugins
{
    public class AssemblyManager
    {
        private readonly HttpClient _httpClient;

        public readonly SemaphoreSlim PluginLock;

        private List<Assembly> _assemblies;

        public AssemblyManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _assemblies = new List<Assembly>();
            PluginLock = new SemaphoreSlim(1, 1);
        }

        /// <summary>
        /// Download a plugin assembly from the url and load it as an assembly.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<Assembly> LoadAssembly(string url)
        {
            var assemblyBytes = await _httpClient.GetByteArrayAsync(url);

            var assembly = Assembly.Load(assemblyBytes);

            _assemblies.Add(assembly);

            return assembly;
        }

        public async Task<IEnumerable<Assembly>> GetAssemblies()
        {
            await PluginLock.WaitAsync();

            var assemblies = _assemblies;

            PluginLock.Release();

            return assemblies;
        }
    }
}
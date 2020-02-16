using System.Threading.Tasks;
using AppDash.Client.Web;
using Microsoft.AspNetCore.Blazor.Hosting;

namespace AppDash.Client.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            await builder.Build().RunAsync();
        }
    }
}

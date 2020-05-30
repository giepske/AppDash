using AppDash.Plugins.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace MinecraftPlugin.Controllers
{
    public class TestController : PluginController<MinecraftPlugin>
    {
        [PluginControllerRoute("test")]
        public IActionResult TestServer([FromBody]Server server)
        {
            var result = new MinecraftServerHelper().PingServer(server.Host, server.Port);

            return new JsonResult(new
            {
                IsSuccess = result != null,
                StatusResponse = result
            });
        }
    }
}

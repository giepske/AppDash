using AppDash.Plugins.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace TorrentPlugin.RTorrent.Controllers
{
    public class AuthController : PluginController<RTorrentPlugin>
    {
        [PluginControllerRoute("rTorrent/test")]
        public IActionResult Login([FromBody]JObject jObject)
        {
            bool result = RTorrentHelper.TestConnection(jObject["Host"].Value<string>(), jObject["Port"].Value<int>());

            return new JsonResult(new
            {
                IsSuccess = result
            });
        }
    }
}

using AppDash.Plugins.Controllers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace TorrentPlugin.QBittorrent.Controllers
{
    public class AuthController : PluginController<QBitTorrentPlugin>
    {
        [PluginControllerRoute("qBittorrent/login")]
        public IActionResult Login([FromBody]JObject jObject)
        {
            bool result = QBittorrentHelper.TryLogin(jObject["Host"].Value<string>(), 
                jObject["Username"].Value<string>(), 
                jObject["Password"].Value<string>(), 
                out string cookieValue);

            return new JsonResult(new
            {
                IsSuccess = result,
                LoginCookie = cookieValue
            });
        }
    }
}

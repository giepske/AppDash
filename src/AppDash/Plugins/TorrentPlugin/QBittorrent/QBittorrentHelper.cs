using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AppDash.Core;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;

namespace TorrentPlugin.QBittorrent
{
    public static class QBittorrentHelper
    {
        private const string ApiUrl = "api/v2/";

        public static bool TryLogin(string host, string username, string password, out string cookieValue)
        {
            cookieValue = null;

            if (!host.EndsWith("/"))
                host += "/";

            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var result = httpClient.PostAsync(host + $"{ApiUrl}auth/login",
                        new FormUrlEncodedContent(new []
                        {
                            new KeyValuePair<string, string>("username", username),
                            new KeyValuePair<string, string>("password", password)
                        })).Result;

                    if (result.Headers.TryGetValues("Set-Cookie", out var setCookie))
                    {
                        cookieValue = string.Join(";", setCookie);
                    }

                }
                catch (Exception)
                {
                    return false;
                }
            }

            return !string.IsNullOrEmpty(cookieValue);
        }

        public static async Task<string> GetCurrentStats(string host, string loginCookie)
        {
            if (!host.EndsWith("/"))
                host += "/";

            using (var handler = new HttpClientHandler { UseCookies = false })
            using (HttpClient httpClient = new HttpClient(handler))
            {
                try
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, host + $"{ApiUrl}transfer/info");
                    requestMessage.Headers.Add("Cookie", loginCookie);

                    var response = await httpClient.SendAsync(requestMessage);

                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
    }
}

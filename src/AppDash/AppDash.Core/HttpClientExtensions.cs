using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppDash.Core
{
    public static class HttpClientExtensions
    {
        public static async Task<T> PostJson<T>(this HttpClient httpClient, string url, object data) =>
            await httpClient.SendJson<T>(HttpMethod.Post, url, data);

        public static async Task<T> PutJson<T>(this HttpClient httpClient, string url, object data) =>
            await httpClient.SendJson<T>(HttpMethod.Put, url, data);

        public static async Task<T> PatchJson<T>(this HttpClient httpClient, string url, object data) =>
            await httpClient.SendJson<T>(new HttpMethod("PATCH"), url, data);

        public static async Task<T> GetJson<T>(this HttpClient httpClient, string url) =>
            await httpClient.SendJson<T>(HttpMethod.Get, url, null);

        private static async Task<T> SendJson<T>(this HttpClient httpClient, HttpMethod method, string url, object data)
        {
            var response = await httpClient.SendAsync(new HttpRequestMessage(method, url)
            {
                Content = data == null ?
                    null :
                    new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
            });

            var stringContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(stringContent);
        }
    }
}


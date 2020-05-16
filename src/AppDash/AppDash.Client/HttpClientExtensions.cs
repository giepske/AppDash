using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppDash.Client
{
    public static class HttpClientExtensions
    {
        public static async Task<T> PostJsonAsync<T>(this HttpClient httpClient, string url, object data) =>
            await httpClient.SendJsonAsync<T>(HttpMethod.Post, url, data);

        public static async Task<T> PutJsonAsync<T>(this HttpClient httpClient, string url, object data) =>
            await httpClient.SendJsonAsync<T>(HttpMethod.Put, url, data);

        public static async Task<T> GetJsonAsync<T>(this HttpClient httpClient, string url) =>
            await httpClient.SendJsonAsync<T>(HttpMethod.Get, url, null);

        private static async Task<T> SendJsonAsync<T>(this HttpClient httpClient, HttpMethod method, string url, object data)
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


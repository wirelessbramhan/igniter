using System.Net.Http;
using System.Threading.Tasks;
using Gpp.Log;
using Newtonsoft.Json;

namespace Gpp.Utils
{
    internal static class HttpUtil
    {
        private static readonly HttpClient client = new HttpClient();

        internal static T GetJson<T>(string url)
        {
            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                string json = response.Content.ReadAsStringAsync().Result;
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
            catch (HttpRequestException e)
            {
                GppLog.Log($"HTTP 요청 오류: {e.Message}");
                throw;
            }
        }

        internal static async Task<T> GetJsonAsync<T>(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                T result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
            catch (HttpRequestException e)
            {
                GppLog.Log($"HTTP 요청 오류: {e.Message}");
                throw;
            }
        }
    }
}
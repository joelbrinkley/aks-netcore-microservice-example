using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FrontEnd.Services
{
    public static class HttpResponseExtension
    {
        public async static Task<ServiceResponse<T>> ParseResponse<T>(this HttpResponseMessage response)
        {
            string errorMessage = string.Empty;
            T result = default(T);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                JObject obj = JObject.Parse(responseContent);
                errorMessage = obj["message"]?.ToString();
            }
            else
            {
                result = JsonConvert.DeserializeObject<T>(responseContent);
            }

            return new ServiceResponse<T>()
            {
                Successful = response.IsSuccessStatusCode,
                ErrorMessage = errorMessage,
                Result = result
            };
        }
    }
}
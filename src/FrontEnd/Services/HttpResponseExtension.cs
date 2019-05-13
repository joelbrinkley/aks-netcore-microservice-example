using System;
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
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"StatusCode: {response.StatusCode}");
            Console.WriteLine(responseContent);
            string errorMessage = string.Empty;
            T result = default(T);


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
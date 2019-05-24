using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Communications.Messages;
using FrontEnd.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FrontEnd.Services
{
    public class CommunicationsService : IHttpService
    {
        private HttpClient client;

        public CommunicationsService(HttpClient client, IOptions<CommunicationsServiceOptions> options)
        {
            if (string.IsNullOrEmpty(options.Value.BaseUri)) throw new ArgumentException("CommunicationsApi BaseUri cannot be null or empty");

            this.client = client;
            this.client.BaseAddress = new Uri(options.Value.BaseUri);
            this.client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<ServiceResponse<string>> SendCommunication(SendCommunicationCommand model)
        {

            var json = JsonConvert.SerializeObject(model);
            var response = await client.PostAsync("/communications", new StringContent(json, Encoding.UTF8, "application/json"));
            var result = await response.ParseResponse<string>();
            return result;
        }
    }
}
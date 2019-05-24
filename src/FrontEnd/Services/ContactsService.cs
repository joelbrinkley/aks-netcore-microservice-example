using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Contacts.Commands;
using FrontEnd.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FrontEnd.Services
{
    public class ContactsService : IHttpService
    {
        private HttpClient client;

        public ContactsService(HttpClient client, IOptions<ContactServiceOptions> options)
        {
            if (string.IsNullOrEmpty(options.Value.BaseUri)) throw new ArgumentException("ContactsService BaseUri cannot be null or empty");

            this.client = client;
            this.client.BaseAddress = new Uri(options.Value.BaseUri);
            this.client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

        }

        public async Task<ServiceResponse<ContactViewModel>> RemoveContact(string email)
        {
            var response = await client.DeleteAsync($"/contacts/{email}");
            var result = await response.ParseResponse<ContactViewModel>();
            return result;
        }

        public async Task<ServiceResponse<IEnumerable<ContactViewModel>>> GetContacts()
        {
            var response = await this.client.GetAsync("/contacts");
            var result = await response.ParseResponse<IEnumerable<ContactViewModel>>();
            return result;
        }

        public async Task<ServiceResponse<ContactViewModel>> Add(ContactViewModel model)
        {
            var newContactCommand = new NewContactCommand(model.FirstName, model.LastName, model.EmailAddress);
            var json = JsonConvert.SerializeObject(newContactCommand);
            var response = await client.PostAsync("/contacts", new StringContent(json, Encoding.UTF8, "application/json"));
            var result = await response.ParseResponse<ContactViewModel>();
            return result;
        }
    }
}

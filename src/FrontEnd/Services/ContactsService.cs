using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FrontEnd.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FrontEnd.Services
{
    public class ContactsService
    {
        private HttpClient client;
        public ContactsService(HttpClient client, IOptions<ContactsServiceOptions> options)
        {
            if(string.IsNullOrEmpty(options.Value.BaseUri)) throw new ArgumentException("ContactsService BaseUri cannot be null or empty");

            this.client = client;
            this.client.BaseAddress = new Uri(options.Value.BaseUri);

        }

        public async Task<List<ContactViewModel>> GetContacts()
        {
            var response = await client.GetAsync("/contacts");
            return await response.Content.ReadAsAsync<List<ContactViewModel>>();
        }

        public async Task RemoveContact(string id)
        {
           await client.DeleteAsync("/contact/{id}");
        }

        public async Task<ContactViewModel> Add(ContactViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            var response = await client.PostAsync("/contacts", new StringContent(json, Encoding.UTF8));

            var addedContact = JsonConvert.DeserializeObject<ContactViewModel>(response.Content.ToString());

            return addedContact;
        }
    }
}

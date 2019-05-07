using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FrontEnd.Models;

namespace FrontEnd.Services
{
    public class ContactsService
    {
        private HttpClient client;
        public ContactsService(HttpClient client, string baseUri)
        {
            this.client = client;
            this.client.BaseAddress = new Uri(baseUri);

        }

        public async Task<List<ContactViewModel>> GetContacts()
        {
            var response = await client.GetAsync("/contacts");
            return await response.Content.ReadAsAsync<List<ContactViewModel>>();
        }
    }
}

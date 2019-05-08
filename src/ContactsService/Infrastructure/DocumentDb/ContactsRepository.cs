using System;
using System.Threading.Tasks;
using ContactsService.Models;
using Microsoft.Azure.Cosmos;

namespace ContactsService.Repository
{
    public class ContactsRepository : IContactRepository
    {
        private readonly CosmosContainer container;

        public ContactsRepository(CosmosContainer container)
        {
            this.container = container;
        }

        public async Task<Contact> Add(Contact contact)
        {
            var response = await this.container.Items.CreateItemAsync<Contact>(contact.EmailAddress.ToString(), contact);
            var newContact = await this.container.Items.ReadItemAsync<Contact>(contact.EmailAddress.ToString(), contact.EmailAddress.ToString());
            return newContact;
        }
        public async Task Remove(Contact contact)
        {
            await this.container.Items.DeleteItemAsync<Contact>(contact.EmailAddress.ToString(), contact.EmailAddress.ToString());
        }

        public async Task<Contact> FindAsync(string emailAddress)
        {
            var contact = await this.container.Items.ReadItemAsync<Contact>(emailAddress, emailAddress);
            return contact;
        }
    }
}
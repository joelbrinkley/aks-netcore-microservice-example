using System;
using System.Threading.Tasks;
using ContactsService.Models;
using Microsoft.Azure.Cosmos;

namespace ContactsService.Repository
{
    public class ContactsRepository : IContactRepository
    {
        public const string PartitionKey = "/contactid";

        private readonly CosmosContainer container;

        public ContactsRepository(CosmosContainer container)
        {
            this.container = container;
        }

        public async Task<Contact> Add(Contact contact)
        {
            var response = await this.container.Items.CreateItemAsync<Contact>(PartitionKey, contact);
            var newContact = await this.container.Items.ReadItemAsync<Contact>(PartitionKey, contact.Id);
            return newContact;
        }
        public async Task Remove(Contact contact)
        {
            await this.container.Items.DeleteItemAsync<Contact>(PartitionKey, contact.Id);
        }

        public async Task<Contact> FindAsync(string id)
        {
            var contact = await this.container.Items.ReadItemAsync<Contact>(PartitionKey, id);
            return contact;
        }
    }
}
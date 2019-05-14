using System;
using System.Threading.Tasks;
using ContactsService.Core;
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
            var response = await this.container.Items.CreateItemAsync<Contact>(contact.EmailAddress, contact);
            var newContact = await this.container.Items.ReadItemAsync<Contact>(contact.EmailAddress, contact.Id);
            return newContact;
        }
        public async Task Remove(Contact contact)
        {
            await this.container.Items.DeleteItemAsync<Contact>(contact.EmailAddress, contact.Id);
        }

        public async Task<Contact> FindAsync(string emailAddress)
        {
            var query = new CosmosSqlQueryDefinition("SELECT * FROM Contacts c WHERE c.EmailAddress = @email")
                .UseParameter("@email", emailAddress);

            var paramIterator = container.Items.CreateItemQuery<Contact>(query, emailAddress);

            while (paramIterator.HasMoreResults)
            {
                CosmosQueryResponse<Contact> results = await paramIterator.FetchNextSetAsync();

                foreach (Contact result in results)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
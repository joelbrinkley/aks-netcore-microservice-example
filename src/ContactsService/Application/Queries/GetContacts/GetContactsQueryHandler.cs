using System.Collections.Generic;
using System.Threading.Tasks;
using ContactsService.Core;
using ContactsService.Repository;
using Microsoft.Azure.Cosmos;

namespace ContactsService.Queries
{
    public class GetContactsQueryHandler
    {
        private readonly CosmosContainer container;

        public GetContactsQueryHandler(CosmosContainer container)
        {
            this.container = container;
        }

        public async Task<IEnumerable<Contact>> Execute(GetContactsQuery query)
        {
            CosmosResultSetIterator<Contact> queryResultSetIterator = this.container.Items.GetItemIterator<Contact>();

            List<Contact> contacts = new List<Contact>();

            while (queryResultSetIterator.HasMoreResults)
            {
                CosmosQueryResponse<Contact> currentResultSet = await queryResultSetIterator.FetchNextSetAsync();
                foreach (Contact family in currentResultSet)
                {
                    contacts.Add(family);
                }
            }
            return contacts;
        }
    }
}
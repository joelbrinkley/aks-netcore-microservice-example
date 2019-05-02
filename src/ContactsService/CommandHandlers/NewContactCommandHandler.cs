using System.Threading.Tasks;
using ContactsService.Commands;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ContactsService.CommandHandlers
{
    public class NewContactCommandHandler
    {
        private readonly DocumentClient client;

        public NewContactCommandHandler(DocumentClient client)
        {
            this.client = client;
        }

        public async Task Handle(NewContactCommand command)
        {
             await this.client.CreateDatabaseIfNotExistsAsync(new Database { Id = "Contacts" });
        }
    }
}
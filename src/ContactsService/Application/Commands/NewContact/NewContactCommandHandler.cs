using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Core;
using ContactsService.Infrastructure.Entityframework;
using Newtonsoft.Json;
using ContactsService.Infrastructure;
using System;

namespace ContactsService.Commands
{
    public class NewContactCommandHandler
    {
        private readonly ContactsContext context;

        public NewContactCommandHandler(ContactsContext context)
        {
            this.context = context;
        }

        /// Handler is idempotent. Could be called multiple times due to try logic
        public async Task<Contact> Handle(NewContactCommand command)
        {
            var existingContact = await context.Contacts.FindAsync(command.EmailAddress);

            if (existingContact != null) return existingContact;

            var contact = Contact.Create(command.FirstName, command.LastName, command.EmailAddress);

            context.Contacts.Add(contact);

            context.Notifications.Add(new Notification()
            {
                Id = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                Type = "ContactAddedEvent",
                Data = JsonConvert.SerializeObject(contact)
            });

            await context.SaveChangesAsync();

            return contact;
        }
    }
}
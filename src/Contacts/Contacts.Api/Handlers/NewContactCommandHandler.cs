using System.Threading.Tasks;
using System;
using Contacts.DataAccess;
using Contacts.Commands;
using Contacts.Domain;
using System.Linq;
using Contacts.Api.EventExtensions;

namespace Contacts.Api.Handlers
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

            var notifications = contact.DomainEvents.Select(x => x.ToNotification());

            context.Contacts.Add(contact);

            context.Notifications.AddRange(notifications);

            await context.SaveChangesAsync();

            return contact;
        }
    }
}
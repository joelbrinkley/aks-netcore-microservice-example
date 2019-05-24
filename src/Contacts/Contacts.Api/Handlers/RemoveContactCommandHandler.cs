using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using Contacts.Commands;
using Contacts.DataAccess;
using Contacts.Domain;
using System.Linq;
using Contacts.Api.EventExtensions;

namespace Contacts.Api.Handlers
{
    public class RemoveContactCommandHandler
    {
        private readonly ContactsContext context;

        public RemoveContactCommandHandler(ContactsContext context)
        {
            this.context = context;
        }

        public async Task Handle(RemoveContactCommand command)
        {
            var existingContact = await context.Contacts.FindAsync(command.EmailAddress);

            if (existingContact == null) return;

            Contact.Remove(existingContact);

            this.context.Contacts.Remove(existingContact);

            var notifications = existingContact.DomainEvents.Select(x => x.ToNotification());

            this.context.Notifications.AddRange(notifications);


            await context.SaveChangesAsync();
        }
    }
}
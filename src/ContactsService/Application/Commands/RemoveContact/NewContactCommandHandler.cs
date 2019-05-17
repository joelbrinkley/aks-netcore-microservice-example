using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Core;
using ContactsService.Infrastructure.Entityframework;
using ContactsService.Infrastructure;
using System;
using Newtonsoft.Json;

namespace ContactsService.Commands
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

            context.Contacts.Remove(existingContact);

            context.Notifications.Add(new Notification()
            {
                Id = Guid.NewGuid(),
                OccurredOn = DateTime.UtcNow,
                Type = "ContactRemovedEvent",
                Data = JsonConvert.SerializeObject(existingContact)
            });

            await context.SaveChangesAsync();
        }
    }
}
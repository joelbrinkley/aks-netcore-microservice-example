using System;

namespace ContactsService.Commands
{
    public class RemoveContactCommand
    {
        public string ContactId { get; }
        public DateTime CreatedOn { get; }

        public RemoveContactCommand(string contactId)
        {
            this.ContactId = contactId;
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
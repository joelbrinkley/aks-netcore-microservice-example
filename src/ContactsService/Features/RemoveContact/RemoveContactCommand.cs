using System;

namespace ContactsService.Features
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
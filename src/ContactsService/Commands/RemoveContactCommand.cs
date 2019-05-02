using System;

namespace ContactsService.Commands
{
    public class RemoveContactCommand
    {
        public Guid ContactId { get; }
        public DateTime CreatedOn { get; }

        public RemoveContactCommand(Guid contactId)
        {
            this.ContactId = contactId;
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
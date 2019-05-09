using System;

namespace ContactsService.Commands
{
    public class RemoveContactCommand
    {
        public string EmailAddress { get; }
        public DateTime CreatedOn { get; }

        public RemoveContactCommand(string emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
using System;

namespace ContactsService.Commands
{
    public class NewContactCommand
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string EmailAddress { get; }
        public DateTime CreatedOn { get; }
        public NewContactCommand(string firstName, string lastName, string emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.LastName = lastName;
            this.FirstName = firstName;
            this.CreatedOn = DateTime.UtcNow;

        }
    }
}
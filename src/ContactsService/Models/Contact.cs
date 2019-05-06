using System;
using ContactsService.Exceptions;

namespace ContactsService.Models
{
    public class Contact
    {
        public string Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public Email EmailAddress { get; set; }

        public Contact(string id, string firstName, string lastName, Email emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.LastName = lastName;
            Id = id;
            this.FirstName = firstName;
        }

        public static Contact Create(string firstName, string lastName, string emailAddress)
        {
            if(string.IsNullOrEmpty(firstName)) throw new ContactException("First name is required.");
            if(string.IsNullOrEmpty(lastName)) throw new ContactException("Last name is required.");

            return new Contact(Guid.NewGuid().ToString(),
                               firstName,
                               lastName,
                               new Email(emailAddress));
        }
    }
}
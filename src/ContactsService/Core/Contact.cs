using System;
using ContactsService.Exceptions;
using Newtonsoft.Json;

namespace ContactsService.Core
{
    public class Contact
    {
        public string EmailAddress { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        private Contact()
        {

        }

        public Contact(string firstName, string lastName, string emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.LastName = lastName;
            this.FirstName = firstName;
        }

        public static Contact Create(string firstName, string lastName, string emailAddress)
        {
            if (string.IsNullOrEmpty(firstName)) throw new ContactCreationException("First name is required.");
            if (string.IsNullOrEmpty(lastName)) throw new ContactCreationException("Last name is required.");
            if (!EmailValidator.Validate(emailAddress)) throw new InvalidEmailException(emailAddress);

            return new Contact(firstName,
                               lastName,
                               emailAddress);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
using System;
using ContactsService.Exceptions;
using Newtonsoft.Json;

namespace ContactsService.Models
{
    public class Contact
    {
        public string FirstName { get; }
        public string LastName { get; }
        
        [JsonProperty("id")]
        public Email EmailAddress { get; set; }

        public Contact(string firstName, string lastName, Email emailAddress)
        {
            this.EmailAddress = emailAddress;
            this.LastName = lastName;
            this.FirstName = firstName;
        }

        public static Contact Create(string firstName, string lastName, string emailAddress)
        {
            if (string.IsNullOrEmpty(firstName)) throw new ContactException("First name is required.");
            if (string.IsNullOrEmpty(lastName)) throw new ContactException("Last name is required.");

            return new Contact(firstName,
                               lastName,
                               new Email(emailAddress));
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
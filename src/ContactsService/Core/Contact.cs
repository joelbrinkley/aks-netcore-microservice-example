using System;
using ContactsService.Exceptions;
using Newtonsoft.Json;

namespace ContactsService.Core
{
    public class Contact
    {
        [JsonProperty("id")]
        public string Id { get; }
        public string EmailAddress { get; set; }
        public string FirstName { get; }
        public string LastName { get; }


        public Contact(string id, string firstName, string lastName, string emailAddress)
        {
            this.Id = id;
            this.EmailAddress = emailAddress;
            this.LastName = lastName;
            this.FirstName = firstName;
        }

        public static Contact Create(string firstName, string lastName, string emailAddress)
        {
            if (string.IsNullOrEmpty(firstName)) throw new ContactCreationException("First name is required.");
            if (string.IsNullOrEmpty(lastName)) throw new ContactCreationException("Last name is required.");
            if (!EmailValidator.Validate(emailAddress)) throw new InvalidEmailException(emailAddress);

            return new Contact(Guid.NewGuid().ToString(),
                               firstName,
                               lastName,
                               emailAddress);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
using System;
using System.Collections.Generic;
using Contacts.Domain.Events;
using Domain;

namespace Contacts.Domain
{
    public class Contact : Entity
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


            var newContact =
                new Contact(firstName,
                            lastName,
                            emailAddress);

            newContact.AddEvent(new ContactCreated(newContact));

            return newContact;
        }

        public static void Remove(Contact contact)
        {
            contact.AddEvent(new ContactRemoved(contact));
        }
    }
}
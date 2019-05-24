using System;
using Domain;

namespace Contacts.Domain.Events
{
    public class ContactCreated : DomainEvent
    {
        public Contact Contact { get; }

        public ContactCreated(Contact contact) : base()
        {
            Contact = contact;
        }
    }
}
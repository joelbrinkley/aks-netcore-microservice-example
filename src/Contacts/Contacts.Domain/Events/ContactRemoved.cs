using Domain;

namespace Contacts.Domain.Events
{
    public class ContactRemoved : DomainEvent
    {

        public Contact Contact { get; }

        public ContactRemoved(Contact contact) : base()
        {
            Contact = contact;
        }
    }
}
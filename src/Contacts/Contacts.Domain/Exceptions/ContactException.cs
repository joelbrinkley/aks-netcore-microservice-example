using System;

namespace Contacts.Domain
{
    public class ContactCreationException : ContactServiceException
    {
        public ContactCreationException(string message) : base(message)
        {
        }
    }
}
using System;

namespace Contacts.Domain
{
    public class ContactAlreadyExistsException : ContactServiceException
    {
        public ContactAlreadyExistsException(string email) : base($"A contact already exists for the email address, {email}")
        {
        }
    }
}
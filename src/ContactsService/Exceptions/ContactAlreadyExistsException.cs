using System;

namespace ContactsService.Exceptions
{
    public class ContactAlreadyExistsException : Exception
    {
        public ContactAlreadyExistsException(string email) : base($"A contact already exists for the email address, {email}")
        {
        }
    }
}
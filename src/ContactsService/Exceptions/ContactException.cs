using System;

namespace ContactsService.Exceptions
{
    public class ContactCreationException : ContactServiceException
    {
        public ContactCreationException(string message) : base(message)
        {
        }
    }
}
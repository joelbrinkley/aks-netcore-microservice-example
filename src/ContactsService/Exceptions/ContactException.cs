using System;

namespace ContactsService.Exceptions
{
    public class ContactException : ContactServiceException
    {
        public ContactException(string message) : base(message)
        {
        }
    }
}
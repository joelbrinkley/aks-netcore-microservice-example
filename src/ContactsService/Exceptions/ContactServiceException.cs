using System;

namespace ContactsService.Exceptions
{
    public class ContactServiceException : Exception
    {
        public ContactServiceException()
        {
            
        }
        public ContactServiceException(string message) : base(message)
        {
        }
    }
}
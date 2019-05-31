using System;

namespace Contacts.Domain
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
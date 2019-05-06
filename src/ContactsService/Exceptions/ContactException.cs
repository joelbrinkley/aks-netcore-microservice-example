using System;

namespace ContactsService.Exceptions
{
    public class ContactException : Exception
    {
        public ContactException(string message) : base(message)
        {
        }
    }
}
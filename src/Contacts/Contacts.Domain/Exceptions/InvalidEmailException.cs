using System;

namespace Contacts.Domain
{
    public class InvalidEmailException : ContactServiceException
    {
        public InvalidEmailException(string email) : base($"{email} is not a valid email format.")
        {
        }
    }
}
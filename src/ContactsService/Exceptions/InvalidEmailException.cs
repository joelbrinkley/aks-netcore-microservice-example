using System;

namespace ContactsService.Exceptions
{
    public class InvalidEmailException : ContactServiceException
    {
        public InvalidEmailException(string email) : base($"{email} is not a valid email format.")
        {
        }
    }
}
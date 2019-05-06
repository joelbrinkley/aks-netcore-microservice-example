using System;

namespace ContactsService.Exceptions
{
    public class InvalidEmailException : Exception
    {
        public InvalidEmailException(string email) : base($"{email} is not a valid email format.")
        {
        }
    }
}
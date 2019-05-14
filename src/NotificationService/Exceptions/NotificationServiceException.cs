using System;

namespace NotificationService.Exceptions
{
    public class NotificationServiceException : Exception
    {
        public NotificationServiceException(string message) : base(message)
        {
        }
    }
}
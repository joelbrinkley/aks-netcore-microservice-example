using NotificationService.Core;

namespace NotificationService.Exceptions
{
    public class InvalidMessageException : NotificationServiceException
    {
        public InvalidMessageException(string message) : base(message)
        {
        }
    }
}
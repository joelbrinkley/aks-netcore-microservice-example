using System;

namespace NotificationService.Commands
{
    public class SendNotificationCommand
    {
        public string Message { get; }
        public DateTime CreatedOn { get; }
        public SendNotificationCommand(string message)
        {
            this.Message = message;
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
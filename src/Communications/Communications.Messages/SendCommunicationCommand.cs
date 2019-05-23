using System;

namespace Communications.Messages
{
    public class SendCommunicationCommand
    {
        public string Message { get; }
        public DateTime CreatedOn { get; }
        public SendCommunicationCommand(string message)
        {
            this.Message = message;
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
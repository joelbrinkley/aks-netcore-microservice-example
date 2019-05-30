using System;

namespace Communications.Messages
{
    public class SendCommunicationCommand
    {
        public string Message { get; set;}
        public DateTime CreatedOn { get; }

        public SendCommunicationCommand()
        {
            this.CreatedOn = DateTime.UtcNow;
        }
    }
}
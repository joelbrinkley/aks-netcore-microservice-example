using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NotificationService.Core;
using NotificationService.Exceptions;

namespace NotificationService.Commands
{
    public class SendNotificationCommandHandler
    {
        private readonly QueueClient queueClient;

        public SendNotificationCommandHandler(QueueClient queueClient)
        {
            this.queueClient = queueClient;
        }

        public async Task Handle(SendNotificationCommand command)
        {
            AssertMessageExists(command.Message);
            AssertMessageLength(command.Message);

            var message = new Message(Encoding.UTF8.GetBytes(command.Message));
            
            await queueClient.SendAsync(message);

            Console.WriteLine($"Message sent.");
        }

        private void AssertMessageExists(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new InvalidMessageException("A message was not present.");
            }
        }

        private void AssertMessageLength(string message)
        {
            if (message.Length > MessageLength.MaximumLength)
            {
                throw new InvalidMessageException($"The message exceeds the maximum message length of {MessageLength.MaximumLength}.");
            }
        }
    }
}
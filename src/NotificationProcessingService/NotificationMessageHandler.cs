using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;

namespace NotificationProcessingService
{
    public class NotificationMessageHandler
    {
        private readonly QueueClient queueClient;
        private DbContextOptions<NotificationsProcessingContext> options;

        public NotificationMessageHandler(QueueClient queueClient, string dbConnectionString)
        {
            this.queueClient = queueClient;
            var optionsBuilder = new DbContextOptionsBuilder<NotificationsProcessingContext>();
            optionsBuilder.UseSqlServer(dbConnectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                });
            this.options = optionsBuilder.Options;
        }

        public void Start()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.WriteLine("Message Handler Started");
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            //mock long running process
            Thread.SpinWait(2000);

            Console.WriteLine(
                $"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}"
            );

            using (var context = new NotificationsProcessingContext(this.options))
            {
                var contacts = await context.Contacts.ToListAsync();
                
                var emails = contacts.Select(x => x.Email);

                foreach (var email in emails)
                {
                    Console.WriteLine($"Mock sending notification to {email}");
                }
            }

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

    }
}
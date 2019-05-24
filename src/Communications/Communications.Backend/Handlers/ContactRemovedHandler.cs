using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Communications.DataAccess;
using Contacts.Messages.Notifications;

namespace Communications.Backend.Handlers
{
    public class ContactRemovedHandler
    {
        private readonly SubscriptionClient subscriptionClient;
        private DbContextOptions<CommunicationsContext> options;

        public ContactRemovedHandler(SubscriptionClient subscriptionClient, string dbConnectionString)
        {
            this.subscriptionClient = subscriptionClient;

            this.options = OptionsFactory.NewDbOptions<CommunicationsContext>(dbConnectionString);
        }

        public void Start()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.WriteLine("Message Handler Started");
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var json = Encoding.UTF8.GetString(message.Body);

            Console.WriteLine(
                $"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{json}"
            );

            var notificationEvent = JsonConvert.DeserializeObject<ContactRemovedEventNotification>(json);

            using (var context = new CommunicationsContext(this.options))
            {
                var existingContact = await context.Contacts
                    .FirstOrDefaultAsync(x => string.Equals(x.Email, notificationEvent.Email, StringComparison.CurrentCultureIgnoreCase));

                if (existingContact != null)
                {
                    context.Contacts.Remove(existingContact);

                    await context.SaveChangesAsync();
                }
            }

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
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
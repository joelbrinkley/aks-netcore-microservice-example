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
using NotificationProcessingService.Core;
using NotificationProcessingService.EntityFramework;

namespace NotificationProcessingService
{
    public class ContactNotificationHandler
    {
        private readonly SubscriptionClient subscriptionClient;
        private DbContextOptions<NotificationsProcessingContext> options;

        private Dictionary<string, Action<JObject>> eventActions;

        public ContactNotificationHandler(SubscriptionClient subscriptionClient, string dbConnectionString)
        {
            this.subscriptionClient = subscriptionClient;

            this.options = OptionsFactory.NewDbOptions<NotificationsProcessingContext>(dbConnectionString);

            eventActions = new Dictionary<string, Action<JObject>>()
            {
                {"ContactRemovedEvent", async (obj) => { await RemoveContact(obj);}},
                {"ContactAddedEvent"  , async (obj) => { await AddContact(obj);   }}
            };
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

            JObject obj = JObject.Parse(json);

            JObject content = JObject.Parse(obj["Data"]?.ToString());

            eventActions[obj["Type"].ToString()](content);

            await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private async Task AddContact(JObject obj)
        {
            using (var context = new NotificationsProcessingContext(this.options))
            {
                var email = obj["EmailAddress"]?.ToString();
                context.Contacts.Add(new Contact(0, email));
                await context.SaveChangesAsync();
            }
        }

        private async Task RemoveContact(JObject obj)
        {
            using (var context = new NotificationsProcessingContext(this.options))
            {
                var email = obj["EmailAddress"]?.ToString();
                var existingContact = await context.Contacts.FirstOrDefaultAsync(x => string.Equals(x.Email, email, StringComparison.CurrentCultureIgnoreCase));

                if (existingContact != null)
                {
                    context.Contacts.Remove(existingContact);
                    await context.SaveChangesAsync();
                }
            }
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
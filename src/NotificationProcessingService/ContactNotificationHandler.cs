using System;
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

namespace NotificationProcessingService
{
    public class ContactNotificationHandler
    {
        private readonly SubscriptionClient subscriptionClient;
        private DbContextOptions<NotificationsProcessingContext> options;

        public ContactNotificationHandler(SubscriptionClient subscriptionClient, string dbConnectionString)
        {
            this.subscriptionClient = subscriptionClient;
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

            if (string.Equals(obj["Type"].ToString(), "ContactRemovedEvent", StringComparison.CurrentCultureIgnoreCase))
            {
                await RemoveContact(content);
            }
            else if (string.Equals(obj["Type"].ToString(), "ContactAddedEvent", StringComparison.CurrentCultureIgnoreCase))
            {
                await AddContact(content);
            }
            else
            {
                Console.WriteLine("Unable to handle:\r\n" + json);
            }
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
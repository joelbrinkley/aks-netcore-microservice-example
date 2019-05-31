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
using Contacts.Messages.NotificationEvents;
using Communications.Backend.Handlers;
using System.Reflection;

namespace Communications.Backend.Subscriptions
{
    public class ContactsSubscription
    {
        private readonly SubscriptionClient subscriptionClient;
        private readonly DbContextOptions<CommunicationsContext> options;

        IDictionary<Type, Action<string, CommunicationsContext>> handlers;

        public ContactsSubscription(SubscriptionClient subscriptionClient, string dbConnectionString)
        {
            this.subscriptionClient = subscriptionClient;

            this.options = OptionsFactory.NewDbOptions<CommunicationsContext>(dbConnectionString);

            this.handlers = new Dictionary<Type, Action<string, CommunicationsContext>>()
            {
                {
                    typeof(ContactCreatedEventNotification),
                    (notificationJson, context) => {

                        var notificationEvent = JsonConvert.DeserializeObject<ContactCreatedEventNotification>(notificationJson);
                        var handler =  new ContactCreatedHandler(context);
                        handler.Handle(notificationEvent).Wait();
                    }
                },
                {
                     typeof(ContactRemovedEventNotification),
                     (notificationJson, context) => {

                        var notificationEvent = JsonConvert.DeserializeObject<ContactRemovedEventNotification>(notificationJson);
                        var handler =  new ContactRemovedHandler(context);
                        handler.Handle(notificationEvent).Wait();
                    }
                }
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

            var notification = JObject.Parse(json);
            Type notificationType = Assembly.Load("Contacts.Messages").GetType(notification["Type"]?.ToString());

            using (var context = new CommunicationsContext(this.options))
            {
                handlers[notificationType](json, context);
                await context.SaveChangesAsync();
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
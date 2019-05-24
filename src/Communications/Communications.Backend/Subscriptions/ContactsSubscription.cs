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
using Communications.Backend.Handlers;

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
                    async (notificationJson, context) => {

                        var notificationEvent = JsonConvert.DeserializeObject<ContactCreatedEventNotification>(notificationJson);
                        var handler =  new ContactCreatedHandler(context);
                        await handler.Handle(notificationEvent);
                    }
                },
                {
                     typeof(ContactRemovedEventNotification),
                     async (notificationJson, context) => {

                        var notificationEvent = JsonConvert.DeserializeObject<ContactRemovedEventNotification>(notificationJson);
                        var handler =  new ContactRemovedHandler(context);
                        await handler.Handle(notificationEvent);
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
            Type notificationType = Type.GetType(notification["Type"]?.ToString());

            using (var context = new CommunicationsContext(this.options))
            {
                this.handlers[notificationType](json, context);
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
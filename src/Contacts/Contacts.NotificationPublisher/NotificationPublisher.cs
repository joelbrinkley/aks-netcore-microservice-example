using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Contacts.DataAccess;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Messages.NotificationEvents;
using Domain;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace ContactsPublisher
{
    [DisallowConcurrentExecution]
    public class NotificationPublisher : IJob
    {
        private readonly DbContextOptions<ContactsContext> options;
        private readonly string serviceBusConnectionString;
        private readonly string topicName;

        IDictionary<Type, Func<DomainEvent, object>> conversionMap = new Dictionary<Type, Func<DomainEvent, object>>()
        {
           {   typeof(ContactCreated), (@event) => {
                  var contactCreatedEvent = (ContactCreated)@event;
                  return new ContactCreatedEventNotification(contactCreatedEvent.Contact.EmailAddress);
               }
           },
           {   typeof(ContactRemoved), (@event) => {
                  var contactCreatedEvent = (ContactRemoved)@event;
                  return new ContactRemovedEventNotification(contactCreatedEvent.Contact.EmailAddress);
               }
           }
        };

        public NotificationPublisher(string dbConnectionString, string serviceBusConnectionString, string topicName)
        {
            this.topicName = topicName;
            this.serviceBusConnectionString = serviceBusConnectionString;
            this.options = OptionsFactory.NewDbOptions<ContactsContext>(dbConnectionString);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            TopicClient topicClient = new TopicClient(this.serviceBusConnectionString, this.topicName, RetryPolicy.Default);

            Assembly domainAssembly = Assembly.Load("Contacts.Domain");

            using (ContactsContext dbContext = new ContactsContext(options))
            {
                var notificationsToPublish = await dbContext.Notifications.Where(x => x.ProcessedOn == null).ToListAsync();

                Console.WriteLine($"{notificationsToPublish.Count} contact notifications found to publish.");

                foreach (var notification in notificationsToPublish)
                {
                    try
                    {
                        notification.ProcessedOn = DateTime.UtcNow;

                        Type type = domainAssembly.GetType(notification.Type);

                        var domainEvent = JsonConvert.DeserializeObject(notification.Data, type) as DomainEvent;

                        var notificationEvent = this.conversionMap[type](domainEvent);

                        var notificationEventJson = JsonConvert.SerializeObject(notificationEvent);

                        var message = new Message(Encoding.UTF8.GetBytes(notificationEventJson));

                        await topicClient.SendAsync(message);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
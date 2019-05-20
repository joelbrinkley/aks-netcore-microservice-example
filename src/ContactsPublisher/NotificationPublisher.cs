using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly DbContextOptions<ContactPublisherContext> options;
        private readonly string serviceBusConnectionString;
        private readonly string topicName;

        public NotificationPublisher(string connectionString, string serviceBusConnectionString, string topicName)
        {
            this.topicName = topicName;
            this.serviceBusConnectionString = serviceBusConnectionString;
            var optionsBuilder = new DbContextOptionsBuilder<ContactPublisherContext>();
            optionsBuilder.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                });
            this.options = optionsBuilder.Options;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            TopicClient topicClient = new TopicClient(this.serviceBusConnectionString, this.topicName, RetryPolicy.Default);

            using (ContactPublisherContext dbContext = new ContactPublisherContext(options))
            {
                var notificationsToPublish = await dbContext.Notifications.Where(x => x.ProcessedOn == null).ToListAsync();

                Console.WriteLine($"{notificationsToPublish.Count} contact notifications found to publish.");

                foreach (var notification in notificationsToPublish)
                {
                    notification.ProcessedOn = DateTime.UtcNow;
                    var json = JsonConvert.SerializeObject(notification);
                    var message = new Message(Encoding.UTF8.GetBytes(json));
                    await topicClient.SendAsync(message);

                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
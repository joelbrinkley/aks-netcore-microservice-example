using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;

namespace ContactsNotificationPublisher
{
    [DisallowConcurrentExecution]
    public class NotificationPublisher : IJob
    {
        private readonly DbContextOptions<ContactPublisherContext> options;

        public NotificationPublisher(string connectionString)
        {
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
            using (ContactPublisherContext c = new ContactPublisherContext(options))
            {
                Console.WriteLine("Executed");
            }
        }
    }
}
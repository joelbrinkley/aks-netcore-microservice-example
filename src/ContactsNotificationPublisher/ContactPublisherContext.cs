using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ContactsNotificationPublisher
{
    public class ContactPublisherContext : DbContext
    {
        public ContactPublisherContext(DbContextOptions<ContactPublisherContext> options) : base(options)
        {
            Console.WriteLine("New Context");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var implementedConfigTypes =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => !t.IsAbstract
                        && !t.IsGenericTypeDefinition
                        && t.GetTypeInfo().ImplementedInterfaces.Any(i =>
                            i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            foreach (var configType in implementedConfigTypes)
            {
                dynamic config = Activator.CreateInstance(configType);
                modelBuilder.ApplyConfiguration(config);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            Console.WriteLine("disposing context");
        }
    }
}
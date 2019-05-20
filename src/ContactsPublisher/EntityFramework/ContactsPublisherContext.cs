using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace ContactsPublisher
{
    public class ContactPublisherContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }
        
        public ContactPublisherContext(DbContextOptions<ContactPublisherContext> options) : base(options)
        {
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
    }
}
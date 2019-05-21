using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NotificationProcessingService.Core;

namespace NotificationProcessingService
{
    public class NotificationsProcessingContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        
        public NotificationsProcessingContext(DbContextOptions<NotificationsProcessingContext> options) : base(options)
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
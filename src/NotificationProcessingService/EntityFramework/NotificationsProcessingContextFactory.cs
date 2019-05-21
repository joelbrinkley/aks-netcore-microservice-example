using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NotificationProcessingService.EntityFramework
{
   public class NotificationsProcessingContextFactory : IDesignTimeDbContextFactory<NotificationsProcessingContext>
    {
        public NotificationsProcessingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NotificationsProcessingContext>();
            optionsBuilder
            .UseSqlServer("");

            return new NotificationsProcessingContext(optionsBuilder.Options);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CommunicationsProcessor.EntityFramework
{
   public class CommunicationsProcessorContextFactory : IDesignTimeDbContextFactory<CommunicationsContext>
    {
        public CommunicationsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CommunicationsContext>();
            optionsBuilder
            .UseSqlServer("");

            return new CommunicationsContext(optionsBuilder.Options);
        }
    }
}
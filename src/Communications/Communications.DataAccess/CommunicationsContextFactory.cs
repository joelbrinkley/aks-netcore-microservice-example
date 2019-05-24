using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Communications.DataAccess
{
   public class CommunicationsContextFactory : IDesignTimeDbContextFactory<CommunicationsContext>
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
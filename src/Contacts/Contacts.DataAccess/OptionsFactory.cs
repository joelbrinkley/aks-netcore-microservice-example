using System;
using Microsoft.EntityFrameworkCore;

namespace Contacts.DataAccess
{
    public class OptionsFactory
    {
        public static DbContextOptions<T> NewDbOptions<T>(string dbConnectionString) where T : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseSqlServer(dbConnectionString,
              sqlServerOptionsAction: sqlOptions =>
              {
                  sqlOptions.EnableRetryOnFailure(
                  maxRetryCount: 3,
                  maxRetryDelay: TimeSpan.FromSeconds(30),
                  errorNumbersToAdd: null);
              });

            return optionsBuilder.Options;
        }
    }
}
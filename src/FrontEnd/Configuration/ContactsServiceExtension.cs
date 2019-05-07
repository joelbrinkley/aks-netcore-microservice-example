using System;
using FrontEnd.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FrontEnd.Configuration
{
    public static class ContactsServiceExtension
    {
        public static IServiceCollection AddContactsService(this IServiceCollection collection,
       Action<ContactsServiceOptions> setupAction)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            collection.Configure(setupAction);

            collection.AddHttpClient<ContactsService>()
             .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
             .AddPolicyHandler(RetryPolicy.NewRetryPolicy());

            return collection;
        }
    }
}

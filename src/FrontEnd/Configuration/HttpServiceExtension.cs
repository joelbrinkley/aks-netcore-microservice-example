using System;
using FrontEnd.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FrontEnd.Configuration
{
    public static class HttpServiceExtension
    {
        public static IServiceCollection AddHttpService<T,O>(this IServiceCollection collection,
            Action<O> setupAction) where T : class, IHttpService
                                   where O : class
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            collection.Configure(setupAction);

            collection.AddHttpClient<T>()
             .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
             .AddPolicyHandler(RetryPolicy.NewRetryPolicy());

            return collection;
        }
    }
}

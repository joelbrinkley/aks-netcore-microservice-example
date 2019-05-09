using System;
using System.Net.Http;
using Polly;
using Polly.Extensions.Http;

namespace FrontEnd
{
    public static class RetryPolicy
    {
        public static IAsyncPolicy<HttpResponseMessage> NewRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}
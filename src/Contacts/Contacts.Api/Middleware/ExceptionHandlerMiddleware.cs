using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Contacts.Api
{
    public sealed class ExceptionHandlerMiddleware<T>
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            this.next = next;
            logger = loggerFactory.
                    CreateLogger<ExceptionHandlerMiddleware<T>>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            this.logger.LogError($"{exception.Message} \r\n {exception.StackTrace}");
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var message = "An unexpected error occurred.";

            if (exception is T)
            {
                code = (HttpStatusCode)422;
                message = exception.Message;
            }
            else if (exception is Exception)
            {
                code = HttpStatusCode.InternalServerError;

            }

            var result = JsonConvert.SerializeObject(new { message = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;


            this.logger.LogError($"Replying with HttpStatusCode: {code}\r\n {result.ToString()}");
            return context.Response.WriteAsync(result);
        }
    }
}

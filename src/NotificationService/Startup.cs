using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Middleware;
using NotificationService.Exceptions;
using NotificationService.Commands;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotificationService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var serviceBusConnectionString = this.Configuration["ServiceBusConnectionString"]?.ToString();
            var notificationQueueName = this.Configuration["NotificationQueueName"]?.ToString();

            if (string.IsNullOrEmpty(serviceBusConnectionString)) throw new ConfigurationErrorsException("ServiceBusConnectionString is missing.");
            if (string.IsNullOrEmpty(notificationQueueName)) throw new ConfigurationErrorsException("NotificationQueueNameIsMissing");

            services.AddMvc();

            services.AddScoped<QueueClient>(x => new QueueClient(serviceBusConnectionString, notificationQueueName, ReceiveMode.PeekLock));
            services.AddScoped<SendNotificationCommandHandler>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddAzureServiceBusQueue(
                    serviceBusConnectionString,
                    queueName: notificationQueueName,
                    name: "notifications-servicebus-check");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandlerMiddlware<NotificationServiceException>();

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.UseMvc();

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            })
            .UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        }
    }
}

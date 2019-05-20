using System;
using System.Configuration;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NotificationProcessingService
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
            services.AddMvc();

            var serviceBusConnectionString = this.Configuration["ServiceBusConnectionString"]?.ToString();
            var notificationQueueName = this.Configuration["NotificationQueueName"]?.ToString();

            if (string.IsNullOrEmpty(serviceBusConnectionString)) throw new ConfigurationErrorsException("ServiceBusConnectionString is missing.");
            if (string.IsNullOrEmpty(notificationQueueName)) throw new ConfigurationErrorsException("NotificationQueueNameIsMissing");

            services.AddSingleton<QueueClient>(x => new QueueClient(serviceBusConnectionString, notificationQueueName, ReceiveMode.PeekLock));
            services.AddSingleton<NotificationMessageHandler>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddAzureServiceBusQueue(
                    serviceBusConnectionString,
                    queueName: notificationQueueName,
                    name: "notifications-servicebus-check");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifeTime)
        {
            app.UseMvcWithDefaultRoute();

            app
            .UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            })
            .UseHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            var messagehandler = app.ApplicationServices.GetService<NotificationMessageHandler>();
            messagehandler.Start();


            appLifeTime.ApplicationStopping.Register(async () =>
            {
                var qclient = app.ApplicationServices.GetService<QueueClient>();
                if (qclient != null && !qclient.IsClosedOrClosing)
                {
                    await qclient.CloseAsync();
                }
            });
        }
    }
}

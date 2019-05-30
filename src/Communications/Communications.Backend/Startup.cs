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
using Communications.Backend.Handlers;
using Communications.Backend.Subscriptions;
using Communications.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Communications.Backend
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

            var communicationsdbConnectionString = this.Configuration["CommunicationsDbSqlServerConnection"]?.ToString();
            var serviceBusConnectionString = this.Configuration["ServiceBusConnectionString"]?.ToString();
            var communicationsQueueName = this.Configuration["CommunicationsQueueName"]?.ToString();
            var contactTopicName = this.Configuration["ContactsTopic"]?.ToString();
            var subscription = this.Configuration["ContactsTopicSubscription"]?.ToString();

            if (string.IsNullOrEmpty(serviceBusConnectionString)) throw new ConfigurationErrorsException("ServiceBusConnectionString is missing.");
            if (string.IsNullOrEmpty(communicationsQueueName)) throw new ConfigurationErrorsException("CommunicationsQueueName is missing");
            if (string.IsNullOrEmpty(contactTopicName)) throw new ConfigurationErrorsException("ContactTopic is missing");
            if (string.IsNullOrEmpty(subscription)) throw new ConfigurationErrorsException("ContactTopicSubscription is missing");

            //used for creating migrations
            services.AddDbContext<CommunicationsContext>(options =>
            {
                options.UseSqlServer(communicationsdbConnectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                });
            });

            services.AddSingleton(c => new SendCommunicationSubscription(new QueueClient(serviceBusConnectionString, communicationsQueueName, ReceiveMode.PeekLock), communicationsdbConnectionString));
            services.AddSingleton(c => new ContactsSubscription(new SubscriptionClient(serviceBusConnectionString, contactTopicName, subscription, ReceiveMode.PeekLock), communicationsdbConnectionString));

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy())
                .AddAzureServiceBusQueue(
                    serviceBusConnectionString,
                    queueName: communicationsQueueName,
                    name: "communications-servicebus-check");
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

            var sendCommunicationSubscription = app.ApplicationServices.GetService<SendCommunicationSubscription>();
            sendCommunicationSubscription.Start();

            var contactsSubscription = app.ApplicationServices.GetService<ContactsSubscription>();
            contactsSubscription.Start();
        }
    }
}

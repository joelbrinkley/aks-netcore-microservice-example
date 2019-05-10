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
            var notificationQueueConnectionString = this.Configuration["NotificationQueueConnectionString"]?.ToString();
            var notificationQueueName = this.Configuration["NotificationQueueName"]?.ToString();

            if (string.IsNullOrEmpty(notificationQueueConnectionString)) throw new ConfigurationErrorsException("NotificationQueueConnectionString is missing.");
            if (string.IsNullOrEmpty(notificationQueueName)) throw new ConfigurationErrorsException("NotificationQueueNameIsMissing");

            services.AddMvc();

            services.AddScoped<QueueClient>(x => new QueueClient(notificationQueueConnectionString, notificationQueueName, ReceiveMode.ReceiveAndDelete));
            services.AddScoped<SendNotificationCommandHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandlerMiddlware<NotificationServiceException>();

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.UseMvc();
        }
    }
}

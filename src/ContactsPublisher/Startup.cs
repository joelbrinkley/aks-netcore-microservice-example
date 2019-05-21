using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;
using Quartz.Impl;

namespace ContactsPublisher
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var connectionstring = this.Configuration["ContactsDbSqlServerConnection"]?.ToString();
            if (string.IsNullOrEmpty(connectionstring)) throw new ConfigurationErrorsException("ContactsDbSqlServerConnection is missing.");

            var serviceBusConnectionString = this.Configuration["ServiceBusConnectionString"]?.ToString();
            if (string.IsNullOrEmpty(serviceBusConnectionString)) throw new ConfigurationErrorsException("ServiceBusConnectionString is missing.");

            var contactsTopic = this.Configuration["ContactsTopic"]?.ToString();
            if (string.IsNullOrEmpty(contactsTopic)) throw new ConfigurationErrorsException("ContactsTopic is missing.");

            services.AddTransient(c => new NotificationPublisher(connectionstring, serviceBusConnectionString, contactsTopic));

            services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy())
                    .AddSqlServer(connectionString: connectionstring, name: "sqlcheck")
                    .AddAzureServiceBusQueue(
                        serviceBusConnectionString,
                        queueName: contactsTopic,
                        name: "contacts-servicebus-check");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.UseMvc();

            app.UseHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });

            var quartz = new QuartzStartup(app.ApplicationServices);
            applicationLifetime.ApplicationStarted.Register(async () => await quartz.Start());
        }
    }
}

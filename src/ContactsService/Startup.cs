using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ContactsService.Commands;
using ContactsService.Exceptions;
using ContactsService.Queries;
using ContactsService.Repository;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Middleware;

namespace ContactsService
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

            var comsosDbConnectionString = this.Configuration["CosmosdbConnection"]?.ToString();
            if (string.IsNullOrEmpty(comsosDbConnectionString)) throw new ConfigurationErrorsException("CosmosdbConnection is missing.");

            var client = new CosmosClient(comsosDbConnectionString);
            CosmosDatabase db = client.Databases.CreateDatabaseIfNotExistsAsync("Contacts").GetAwaiter().GetResult();
            CosmosContainer contacts = db.Containers.CreateContainerIfNotExistsAsync("Contacts", "/EmailAddress", 400).GetAwaiter().GetResult();

            services.AddSingleton(contacts)
                    .AddScoped<IContactRepository, ContactsRepository>()
                    .AddScoped<NewContactCommandHandler>()
                    .AddScoped<RemoveContactCommandHandler>()
                    .AddScoped<GetContactsQueryHandler>();

            services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy())
                    .AddCheck("cosmosdb", () =>
                    {
                        var serviceProvider = services.BuildServiceProvider();
                        var container = serviceProvider.GetService<CosmosContainer>();
                        var p = container.ReadStreamAsync().GetAwaiter().GetResult();
                        if (p.IsSuccessStatusCode) return HealthCheckResult.Healthy();
                        return HealthCheckResult.Unhealthy();
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandlerMiddlware<ContactServiceException>();

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

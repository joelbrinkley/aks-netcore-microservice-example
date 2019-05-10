using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ContactsService.Commands;
using ContactsService.Exceptions;
using ContactsService.Queries;
using ContactsService.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseExceptionHandlerMiddlware<ContactServiceException>();

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}

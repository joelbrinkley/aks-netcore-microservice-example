using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using ContactsService.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;

namespace ContactsService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var comsosDbConnectionString = ConfigurationManager.ConnectionStrings["CosmosConnection"]?.ToString();
            if(string.IsNullOrEmpty(comsosDbConnectionString)) throw new ConfigurationErrorsException("CosmosConnection is missing.");

            var client = new CosmosClient(comsosDbConnectionString);
            CosmosDatabase db = client.Databases.CreateDatabaseIfNotExistsAsync("Contacts").GetAwaiter().GetResult();
            CosmosContainer contacts = db.Containers.CreateContainerIfNotExistsAsync("Contacts", "/contactid", 400).GetAwaiter().GetResult();
            
            services.AddSingleton(contacts);
            services.AddScoped<IContactRepository, ContactsRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}

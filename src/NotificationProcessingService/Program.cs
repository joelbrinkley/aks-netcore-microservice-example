using System;
using System.Threading;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationProcessingService
{
    class Program
    {
        static ManualResetEvent shutdown = new ManualResetEvent(false);
        static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            NotificationMessageHandler notificationMessageHandler = null;
            QueueClient queueClient = null;

            Configuration = BuildConfig();

            try
            {
                queueClient = GetQueueClient();
                notificationMessageHandler = new NotificationMessageHandler(queueClient);
                notificationMessageHandler.Start();
                shutdown.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                notificationMessageHandler?.Stop();
                queueClient?.CloseAsync().Wait();
            }
            Console.WriteLine("NotificationProcessingService has shutdown.");
        }

        private static IConfiguration BuildConfig()
        {
            var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) ||
                                           devEnvironmentVariable.ToLower() == "development";

            var builder = new ConfigurationBuilder();


            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            if (isDevelopment)
            {
                builder.AddUserSecrets<Program>();
            }

            var builtConfig = builder.Build();

            var isKeyVaultEnabled = builtConfig["AzureKeyVault:Enabled"] == "true";

            if (isKeyVaultEnabled)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                var keyVaultClient = new KeyVaultClient(
                    new KeyVaultClient.AuthenticationCallback(
                        azureServiceTokenProvider.KeyVaultTokenCallback));

                builder.AddAzureKeyVault(
                     $"https://{builtConfig["AzureKeyVault:Name"]}.vault.azure.net/",
                     builtConfig["AzureAD:ClientId"],
                     builtConfig["AzureAD:ClientSecret"],
                     new DefaultKeyVaultSecretManager());
            }

            return builder.Build();
        }

        private static QueueClient GetQueueClient()
        {
            var notificationQueueConnectionString = Configuration["NotificationQueueConnectionString"]?.ToString();
            var queueName = Configuration["NotificationsQueueName"]?.ToString();
            if (string.IsNullOrEmpty(notificationQueueConnectionString)) throw new Exception("NotificationQueueConnectionString is missing.");
            if (string.IsNullOrEmpty(queueName)) throw new Exception("NotificationQueueName is missing.");
            return new QueueClient(notificationQueueConnectionString, queueName);

        }
    }
}

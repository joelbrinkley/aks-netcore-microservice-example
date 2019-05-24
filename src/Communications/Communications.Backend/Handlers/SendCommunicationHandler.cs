using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Communications.Messages;
using Communications.DataAccess;

namespace Communications.Service.Backend
{
    public class SendCommunicationCommandHandler
    {
        private readonly QueueClient queueClient;
        private DbContextOptions<CommunicationsContext> options;

        public SendCommunicationCommandHandler(QueueClient queueClient, string dbConnectionString)
        {
            this.queueClient = queueClient;
            this.options = OptionsFactory.NewDbOptions<CommunicationsContext>(dbConnectionString);
        }

        public void Start()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.WriteLine("Message Handler Started");
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            //mock long running process
            Thread.SpinWait(2000);

            Console.WriteLine(
                $"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}"
            );

            var command = JsonConvert.DeserializeObject<SendCommunicationCommand>(Encoding.UTF8.GetString(message.Body));
            
            using (var context = new CommunicationsContext(this.options))
            {
                var contacts = await context.Contacts.ToListAsync();

                var emails = contacts.Select(x => x.Email);

                foreach (var email in emails)
                {
                    Console.WriteLine($"Mock sending communications to {email}");
                }
            }

            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }

    }
}
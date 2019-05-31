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

namespace Communications.Backend.Handlers
{
    public class SendCommunicationCommandHandler
    {
        private readonly CommunicationsContext context;

        public SendCommunicationCommandHandler(CommunicationsContext context)
        {
            this.context = context;
        }

        public async Task Handle(SendCommunicationCommand command)
        {
            //mock long running process
            Thread.SpinWait(2000);
            
            var contacts = await context.Contacts.ToListAsync();

            var emails = contacts.Select(x => x.Email);

            foreach (var email in emails)
            {
                Console.WriteLine($"Mock sending communications to {email}");
            }
        }
    }
}
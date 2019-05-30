using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Communications.DataAccess;
using Contacts.Messages.NotificationEvents;

namespace Communications.Backend.Handlers
{
    public class ContactRemovedHandler
    {
        private readonly CommunicationsContext context;

        public ContactRemovedHandler(CommunicationsContext context)
        {
            this.context = context;
        }

        public async Task Handle(ContactRemovedEventNotification notification)
        {
            var existingContact = await context.Contacts
                .FirstOrDefaultAsync(x => string.Equals(x.Email, notification.Email, StringComparison.CurrentCultureIgnoreCase));

            if (existingContact != null)
            {
                context.Contacts.Remove(existingContact);
            }
        }
    }
}
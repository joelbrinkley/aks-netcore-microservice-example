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
    public class ContactCreatedHandler
    {
        private readonly CommunicationsContext context;

        public ContactCreatedHandler(CommunicationsContext context)
        {
            this.context = context;
        }

        public Task Handle(ContactCreatedEventNotification notification)
        {
            context.Contacts.Add(new Contact(0, notification.Email));
            return Task.CompletedTask;
        }  
    }
}
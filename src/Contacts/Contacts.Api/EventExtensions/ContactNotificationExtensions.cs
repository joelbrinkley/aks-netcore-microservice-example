using System;
using Contacts.DataAccess;
using Contacts.Domain.Events;
using Contacts.Messages.Notifications;
using Domain;
using Newtonsoft.Json;

namespace Contacts.Api.EventExtensions
{
    public static class EventExtensions
    {
        public static Notification ToNotification(this IDomainEvent @event)
        {
            return new Notification()
            {
                Id =  Guid.NewGuid(),
                Data = JsonConvert.SerializeObject(@event),
                OccurredOn = @event.OccurredOn,
                Type = @event.GetType().ToString()
            };
        }
    }
}
using System;

namespace Contacts.Messages.NotificationEvents
{
    public class ContactCreatedEventNotification : NotificationEvent
    {
        public string Id { get; }
        public string Email { get; }

        public ContactCreatedEventNotification(string email)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Email = email;
        }
    }
}
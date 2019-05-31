using System;

namespace Contacts.Messages.NotificationEvents
{
    public class ContactRemovedEventNotification : NotificationEvent
    {
        public string Id { get; }
        public string Email { get; }

        public ContactRemovedEventNotification(string email)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Email = email;
        }
    }
}
using System;

namespace Contacts.Messages.Notifications
{
    public class ContactCreatedEventNotification
    {
        public string Id { get; }
        public string Email { get; }

        public ContactCreatedEventNotification(string email)
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
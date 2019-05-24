using System;

namespace Contacts.Messages.Notifications
{
    public class ContactRemovedEventNotification
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
namespace Contacts.Messages.NotificationEvents
{
    public abstract class NotificationEvent
    {
        public string Type
        {
            get
            {
                return this.GetType().ToString();
            }
        }

    }
}
namespace NotificationProcessingService.Core
{
    public class Contact
    {
        public int Id { get; private set;}
        public string Email { get; private set;}

        private Contact()
        {

        
        }
        public Contact(int id, string email)
        {
            this.Id = id;
            this.Email = email;
        }
    }
}
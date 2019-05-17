using System.Threading.Tasks;
using ContactsService.Core;
using ContactsService.Repository;

namespace ContactsService.Infrastructure.Entityframework
{
    public class ContactsRepository : IContactsRepository
    {
        private readonly ContactsContext context;

        public ContactsRepository(ContactsContext context)
        {
            this.context = context;
        }
        
        public async Task<Contact> Add(Contact contact)
        {
            await this.context.Contacts.AddAsync(contact);
            return contact;
        }

        public async Task<Contact> FindAsync(string email)
        {
           var contact = await this.context.Contacts.FindAsync(email);
           return contact;
        }

        public async Task Remove(Contact contact)
        {
            var existingContact = await this.FindAsync(contact.EmailAddress);
            if(existingContact != null) this.context.Contacts.Remove(existingContact);
        }
    }
}
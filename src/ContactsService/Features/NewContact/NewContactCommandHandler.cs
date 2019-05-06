using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Infrastructure.DocumentDb;
using ContactsService.Models;

namespace ContactsService.Features
{
    public class NewContactCommandHandler
    {
        private readonly ContactsRepository contactsRepository;

        public NewContactCommandHandler(ContactsRepository contactsRepository)
        {
            this.contactsRepository = contactsRepository;
        }

        public async Task<Contact> Handle(NewContactCommand command)
        {
            var existingContact = await contactsRepository.FindAsync(command.EmailAddress);

            if (existingContact != null) throw new ContactAlreadyExistsException(command.EmailAddress);

            var contact = Contact.Create(command.FirstName, command.LastName, command.EmailAddress);

            return await contactsRepository.Add(contact);
        }
    }
}
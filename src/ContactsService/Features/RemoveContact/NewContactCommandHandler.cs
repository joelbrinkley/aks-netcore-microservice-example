using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Infrastructure.DocumentDb;
using ContactsService.Models;

namespace ContactsService.Features
{
    public class RemoveContactCommandHandler
    {
        private readonly ContactsRepository contactsRepository;

        public RemoveContactCommandHandler(ContactsRepository contactsRepository)
        {
            this.contactsRepository = contactsRepository;
        }

        public async Task Handle(RemoveContactCommand command)
        {
            var existingContact = await contactsRepository.FindAsync(command.ContactId);

            if (existingContact != null) return;

            await contactsRepository.Remove(existingContact);
        }
    }
}
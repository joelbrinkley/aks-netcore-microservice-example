using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Core;
using ContactsService.Repository;

namespace ContactsService.Commands
{
    public class RemoveContactCommandHandler
    {
        private readonly IContactRepository contactsRepository;

        public RemoveContactCommandHandler(IContactRepository contactsRepository)
        {
            this.contactsRepository = contactsRepository;
        }

        public async Task Handle(RemoveContactCommand command)
        {
            var existingContact = await contactsRepository.FindAsync(command.EmailAddress);

            if (existingContact != null) return;

            await contactsRepository.Remove(existingContact);
        }
    }
}
using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Core;
using ContactsService.Repository;
using ContactsService.Infrastructure.Entityframework;

namespace ContactsService.Commands
{
    public class NewContactCommandHandler
    {
        private readonly UnitOfWork unitOfWork;

        public NewContactCommandHandler(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        /// Handler is idempotent. Could be called multiple times due to try logic
        public async Task<Contact> Handle(NewContactCommand command)
        {
            var existingContact = await unitOfWork.ContactsRepository.FindAsync(command.EmailAddress);

            if (existingContact != null) return existingContact;

            var contact = Contact.Create(command.FirstName, command.LastName, command.EmailAddress);

            contact = await unitOfWork.ContactsRepository.Add(contact);

            await unitOfWork.Commit();

            return contact;
        }
    }
}
using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Core;
using ContactsService.Repository;
using ContactsService.Infrastructure.Entityframework;

namespace ContactsService.Commands
{
    public class RemoveContactCommandHandler
    {
        private readonly UnitOfWork unitOfWork;

        public RemoveContactCommandHandler(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task Handle(RemoveContactCommand command)
        {
            var existingContact = await unitOfWork.ContactsRepository.FindAsync(command.EmailAddress);

            if (existingContact == null) return;

            await unitOfWork.ContactsRepository.Remove(existingContact);

            await unitOfWork.Commit();
        }
    }
}
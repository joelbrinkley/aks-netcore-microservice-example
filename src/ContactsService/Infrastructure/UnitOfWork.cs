using System.Linq;
using System.Threading.Tasks;
using ContactsService.Repository;
using Microsoft.EntityFrameworkCore;

namespace ContactsService.Infrastructure.Entityframework
{
    public class UnitOfWork
    {
        public IContactsRepository ContactsRepository { get; }

        private readonly DbContext context;

        public UnitOfWork(ContactsContext context)
        {
            this.context = context;
            this.ContactsRepository = new ContactsRepository(context);
        }

        public async Task<int> Commit()
        {
            return await context.SaveChangesAsync();
        }

        public void Rollback()
        {
            context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
        }
    }
}
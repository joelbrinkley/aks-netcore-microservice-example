using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactsService.Core;
using ContactsService.Infrastructure.Entityframework;
using ContactsService.Repository;
using Microsoft.EntityFrameworkCore;

namespace ContactsService.Queries
{
    public class GetContactsQueryHandler
    {
        private readonly ContactsContext context;

        public GetContactsQueryHandler(ContactsContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Contact>> Execute(GetContactsQuery query)
        {
            return await context.Contacts.ToListAsync();
        }
    }
}
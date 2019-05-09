using System;
using System.Threading.Tasks;
using ContactsService.Exceptions;
using ContactsService.Core;

namespace ContactsService.Repository
{
    public interface IContactRepository
    {
        Task<Contact> Add(Contact contact);
        Task Remove(Contact contact);
        Task<Contact> FindAsync(string id);

    }
}
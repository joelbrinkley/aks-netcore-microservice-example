using System.Text;
using System.Threading.Tasks;
using Contacts.Api.Handlers;
using Contacts.Commands;
using Contacts.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Contacts.Api.Controllers
{
    [Produces("application/json")]
    [Route("contacts")]
    public class ContactsController : Controller
    {
        private readonly ContactsContext context;
        private readonly NewContactCommandHandler newContactCommandHandler;
        private readonly RemoveContactCommandHandler removeContactCommandHandler;

        public ContactsController(
            NewContactCommandHandler newContactCommandHandler,
            RemoveContactCommandHandler removeContactCommandHandler,
            ContactsContext context)
        {
            this.context = context;
            this.newContactCommandHandler = newContactCommandHandler;
            this.removeContactCommandHandler = removeContactCommandHandler;

        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetContacts()
        {
            var contacts = await this.context.Contacts.ToListAsync();
            return Ok(contacts);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> NewContact([FromBody]NewContactCommand command)
        {
             var response = await this.newContactCommandHandler.Handle(command);
             return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveContact(string id)
        {
             await this.removeContactCommandHandler.Handle(new RemoveContactCommand(id));
             return NoContent();
        }
    }
}
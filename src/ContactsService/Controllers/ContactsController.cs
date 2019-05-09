using System.Threading.Tasks;
using ContactsService.Commands;
using ContactsService.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ContactsService.Controllers
{
    [Produces("application/json")]
    [Route("contacts")]
    public class ContactsController : Controller
    {
        private readonly NewContactCommandHandler newContactHandler;
        private readonly RemoveContactCommandHandler removeContactHandler;

        private readonly GetContactsQueryHandler getContactsQueryHandler;
        
        public ContactsController(NewContactCommandHandler newContactHandler,
                                  RemoveContactCommandHandler removeContactHandler,
                                  GetContactsQueryHandler getContactsQueryHandler)
        {
            this.newContactHandler = newContactHandler;
            this.removeContactHandler = removeContactHandler;
            this.getContactsQueryHandler = getContactsQueryHandler;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetContacts()
        {
            var contacts = await this.getContactsQueryHandler.Execute(new GetContactsQuery());
            return Ok(contacts);
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> NewContact([FromBody]NewContactCommand command)
        {
            var result = await this.newContactHandler.Handle(command);
            return Ok(result);
        }

        [HttpDelete]
        [Route("contacts/{id}")]
        public async Task<IActionResult> RemoveContact(string id)
        {
            await this.removeContactHandler.Handle(new RemoveContactCommand(id));
            return NoContent();
        }
    }
}
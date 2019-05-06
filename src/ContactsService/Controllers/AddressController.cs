using System.Threading.Tasks;
using ContactsService.Features;
using Microsoft.AspNetCore.Mvc;

namespace ContactsService.Controllers
{
    [Route("api/addresses")]
    public class AddressController : Controller
    {
        private readonly NewContactCommandHandler newContactHandler;
        private readonly RemoveContactCommandHandler removeContactHandler;

        public AddressController(NewContactCommandHandler newContactHandler,
                                 RemoveContactCommandHandler removeContactHandler)
        {
            this.newContactHandler = newContactHandler;
            this.removeContactHandler = removeContactHandler;
        }

        public async Task<IActionResult> NewContact(NewContactCommand command)
        {
            var result = await this.newContactHandler.Handle(command);
            return Ok(result);
        }

        public async Task<IActionResult> RemoveContact(RemoveContactCommand command)
        {
            await this.removeContactHandler.Handle(command);
            return NoContent();
        }
    }
}
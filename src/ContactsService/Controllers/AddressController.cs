using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ContactsService.Controllers
{
    [Route("api/addresses")]
    public class AddressController : Controller
    {
        public async Task<IActionResult> NewContact()
        {
            return Ok();
        }

        public async Task<IActionResult> RemoveContact()
        {
            return Ok();
        }
    }
}
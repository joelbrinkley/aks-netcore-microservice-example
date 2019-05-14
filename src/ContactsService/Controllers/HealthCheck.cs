using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ContactsService.Controllers
{
    [Route("healthcheck")]
    public class HealthCheck : Controller
    {
        public IActionResult Check()
        {
            return Ok(new { status = "success", message = "Contacts Service up and running..." });
        }
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Controllers
{
    [Route("healthcheck")]
    public class HealthCheck : Controller
    {
        public IActionResult Check()
        {
            return Ok(new { status = "success", message = "Notification Service up and running..." });
        }
    }
}
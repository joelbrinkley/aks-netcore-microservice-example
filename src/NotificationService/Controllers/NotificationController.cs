using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Commands;

namespace NotificationService.Controllers
{
    [Produces("application/json")]
    [Route("notifications")]
    public class NotificationController : Controller
    {
        private readonly SendNotificationCommandHandler sendNotificationCommandHandler;

        public NotificationController(SendNotificationCommandHandler sendNotificationCommandHandler)
        {
            this.sendNotificationCommandHandler = sendNotificationCommandHandler;
        }

        public async Task<IActionResult> SendNotification(SendNotificationCommand command)
        {   
            await this.sendNotificationCommandHandler.Handle(command);
            return Ok();
        }
    }
}
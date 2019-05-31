using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Communications.Messages;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace CommunicationsApi.Controllers
{
    [Produces("application/json")]
    [Route("communications")]
    public class CommunicationsController : Controller
    {
        private readonly QueueClient queueClient;

        public CommunicationsController(QueueClient queueClient)
        {
            this.queueClient = queueClient;
        }

        public async Task<IActionResult> SendCommunication([FromBody]SendCommunicationCommand command)
        {
            var json = JsonConvert.SerializeObject(command);

            var message = new Message(Encoding.UTF8.GetBytes(json));
            
            await queueClient.SendAsync(message);

            return NoContent();
        }
    }
}
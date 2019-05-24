using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FrontEnd.Models;
using FrontEnd.Services;
using Communications.Messages;

namespace FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly CommunicationsService communicationsService;

        public HomeController(CommunicationsService communicationsService)
        {
            this.communicationsService = communicationsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> SendMessage(SendCommunicationCommand model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var response = await communicationsService.SendCommunication(model);

            if (!response.Successful)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage);
                return View("Index", model);
            }

            return View("SendMessage");
        }
    }
}

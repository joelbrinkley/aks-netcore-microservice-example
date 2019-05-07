using System.Collections.Generic;
using FrontEnd.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View("Index", new List<ContactViewModel>());
        }
    }

}
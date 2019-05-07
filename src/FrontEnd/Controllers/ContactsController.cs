using System.Collections.Generic;
using FrontEnd.Models;
using FrontEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontEnd.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ContactsService contactsService;

        public ContactsController(ContactsService contactsService)
        {
            this.contactsService = contactsService;
        }

        public IActionResult Index()
        {
            return View("Index", new List<ContactViewModel>());
        }
    }

}
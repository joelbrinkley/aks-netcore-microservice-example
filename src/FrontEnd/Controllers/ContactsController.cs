using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public IActionResult Create()
        {
            return View("Create", new ContactViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact(ContactViewModel contact)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", contact);
            }

            var response = await contactsService.Add(contact);

            if (!response.Successful)
            {
                ModelState.AddModelError(string.Empty, response.ErrorMessage);
                return View("Create", contact);
            }

            return Redirect("Index");
        }
    }

}
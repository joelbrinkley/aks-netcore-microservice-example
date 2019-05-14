using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models
{
    public class ContactViewModel
    {
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string EmailAddress { get; set; }

    }
}
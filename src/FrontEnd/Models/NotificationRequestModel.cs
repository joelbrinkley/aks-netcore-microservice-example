using System;
using System.ComponentModel.DataAnnotations;
namespace FrontEnd.Models
{
    public class NotificationRequestModel
    {
        [Required]
        public string Message { get; set; }
        public DateTime RequestedOn = DateTime.UtcNow;

    }
}
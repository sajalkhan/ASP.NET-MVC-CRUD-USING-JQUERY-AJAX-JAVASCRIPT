using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPractice1.Models
{
    public class RegistrationViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Enter Name")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Enter a valid mail address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Enter Address")]
        public string Address { get; set; }

        public Nullable<int> RoleId { get; set; }
    }
}
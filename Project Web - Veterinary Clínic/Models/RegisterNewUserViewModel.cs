using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static Project_Web___Veterinary_Clínic.Data.Entities.User;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class RegisterNewUserViewModel
    {

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Address { get; set; }

        [Required]
        //[MaxLength(20, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The field {0} must contain exactly 9 digits.")]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "The field {0} must atleast contain {1} characters.")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string Confirm { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile AvatarFile { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The field {0} must contain exactly 9 digits.")]
        public string Phonenumber { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile AvatarImage { get; set; }

        public string ImageFullPath { get; set; } 
    }
}

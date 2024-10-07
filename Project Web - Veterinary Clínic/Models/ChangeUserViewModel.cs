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
        public string Phonenumber { get; set; }

        [Display(Name = "Profile Picture")]
        public IFormFile AvatarImage { get; set; }

        public string ImageFullPath { get; set; } 
    }
}

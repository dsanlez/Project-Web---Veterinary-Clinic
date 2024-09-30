using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class EditNewVeterinarianViewModel
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
        public string Specialty { get; set; }

        [Required]
        public int? RoomId { get; set; }

        
        public IEnumerable<SelectListItem> Rooms { get; set; }

        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Address { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string PhoneNumber { get; set; }

    }
}

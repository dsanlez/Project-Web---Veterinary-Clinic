using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Data.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Address { get; set; }

        public string Specialty { get; set; }

        public int? RoomId { get; set; }

        public Room Room { get; set; }

        [Display(Name = "Avatar")]
        public string Avatar { get; set; }

        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(Avatar))
                {
                    return null;
                }

                //return $"https://localhost:44363{Avatar.Substring(1)}";
                return $"http://www.VeterinaryClinicSanlez.somee.com{Avatar.Substring(1)}";
            }
        }

    }

}

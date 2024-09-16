using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

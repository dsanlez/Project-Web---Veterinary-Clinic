using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Data.Entities
{
    public class Room : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Name { get; set; } 
        
    }
}

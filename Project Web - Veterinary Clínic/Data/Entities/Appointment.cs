using System.ComponentModel.DataAnnotations;
using System;

namespace Project_Web___Veterinary_Clínic.Data.Entities
{
    public class Appointment : IEntity
    {
        public int Id { get; set; }
        
        public Animal Animal { get; set; }
        public int AnimalId { get; set; }

        public User Veterinarian { get; set; }
        public string VeterinarianId { get; set; }

        public User Customer { get; set; }
        public string CustomerId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm }", ApplyFormatInEditMode = true)]
        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";       
        public Room Room { get; set; }
        public int RoomId { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm }", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int AnimalId { get; set; }
        public IEnumerable<SelectListItem> Animals { get; set; }

        [Required]
        public string VeterinarianId { get; set; }
        public IEnumerable<SelectListItem> Veterinarians { get; set; }

        public string CustomerId { get; set; }
        public IEnumerable<SelectListItem> Customers { get; set; }

        public int RoomId { get; set; }
        public IEnumerable<SelectListItem> Rooms { get; set; }
        public string Status { get; set; }
    }
}

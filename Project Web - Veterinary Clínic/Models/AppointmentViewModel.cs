using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int AnimalId { get; set; }
        public IEnumerable<SelectListItem> Animals { get; set; }

        public string VeterinarianId { get; set; }
        public IEnumerable<SelectListItem> Veterinarians { get; set; }

        public string CustomerId { get; set; }
        public IEnumerable<SelectListItem> Customers { get; set; }

        public int RoomId { get; set; }
        public IEnumerable<SelectListItem> Rooms { get; set; }
    }
}

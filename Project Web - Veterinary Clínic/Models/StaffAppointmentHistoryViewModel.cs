using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class StaffAppointmentHistoryViewModel
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public IEnumerable<SelectListItem> Owners { get; set; }
        public IEnumerable<SelectListItem> Veterinarians { get; set; }
        public IEnumerable<SelectListItem> Animals { get; set; }

        
    }
}

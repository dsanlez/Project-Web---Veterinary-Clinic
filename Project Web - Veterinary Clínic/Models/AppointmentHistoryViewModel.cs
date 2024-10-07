using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class AppointmentHistoryViewModel
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public IEnumerable<Animal> Animals { get; set; }
    }
}

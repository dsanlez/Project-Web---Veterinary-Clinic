using System;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class AppointmentAlertsViewModel
    {
        public int AppointmentId { get; set; }  
        public string AnimalName { get; set; }  
        public string CustomerName { get; set; }
        public string VeterinarianName { get; set; }
        public DateTime AppointmentDate { get; set; }  

        public string Time {  get; set; }
        public string Status { get; set; }  
        public string Message { get; set; } 
    }
}

using Microsoft.AspNetCore.Mvc;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Helpers;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class CommunicationController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        public CommunicationController(IEmailHelper emailHelper, IUserHelper userHelper, IAppointmentRepository appointmentRepository)
        {

            _emailHelper = emailHelper;
            _userHelper = userHelper;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<IActionResult> SendClinicClosureNoticeAsync()
        {
            var customers = await _userHelper.GetCustomersAsync();
            string subject = "Warning: Clinic Closed";
            string body = "Due to unforeseen circumstances, the clinic will be closed until further notice.";

            foreach (var customer in customers)
            {
                _emailHelper.SendEmail(customer.Email, subject, body);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SendAppointmentRemindersAsync()
        {
            var appointments = await _appointmentRepository.GetAppointmentsForTomorrowAsync();
            string subject = "Reminder :Appointment tomorrow!";

            foreach (var appointment in appointments)
            {
                string body = $"Dear {appointment.Customer.FirstName},\nThis is a reminder that you have an appointment scheduled for tomorrow with Dr. {appointment.Veterinarian.FirstName}.";
                _emailHelper.SendEmail(appointment.Customer.Email, subject, body);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CancelAppointmentsByVeterinarianAsync(string veterinarianId)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByVeterinarianAsync(veterinarianId);
            string subject = "Appointment Cancelled";

            foreach (var appointment in appointments)
            {
                string body = $"Dear {appointment.Customer.FirstName},\nUnfortunately, your appointment with Dr. {appointment.Veterinarian.FirstName} has been cancelled.\nWe will get in touch to reschedule the appointment.";
                _emailHelper.SendEmail(appointment.Customer.Email, subject, body);
            }

            return RedirectToAction("Index");
        }
    }
}

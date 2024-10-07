using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_Web___Veterinary_Clínic.Data;
using System.Linq;

namespace Project_Web___Veterinary_Clínic.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public AppointmentsController(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        [HttpGet("animal/{animalId}")]
        public ActionResult GetAppointmentsByAnimal(int animalId)
        {
            var appointments = _appointmentRepository.GetAppointmentsByAnimalId(animalId);
           
            if (appointments == null || !appointments.Any())
            {
                return NotFound($"No appointments found for animal with ID: {animalId}");
            }

            return Ok(appointments);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Helpers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Models;
using System;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAnimalRepository _animalRepository;
        private readonly IUserHelper _userHelper;
        private readonly IRoomRepository _roomRepository;
        private readonly IEmailHelper _emailHelper;

        public AppointmentsController(IAppointmentRepository appointmentRepository,
                                      IAnimalRepository animalRepository,
                                      IUserHelper userHelper,
                                      IRoomRepository roomRepository,
                                      IEmailHelper emailHelper)
        {
            _appointmentRepository = appointmentRepository;
            _animalRepository = animalRepository;
            _userHelper = userHelper;
            _roomRepository = roomRepository;
            _emailHelper = emailHelper;
        }

        // Display all appointments
        public async Task<IActionResult> Index()
        {
            // Fetch all appointments using the generic repository
            var appointments = await _appointmentRepository.GetAllWithUsersAsync();
            return View(appointments);
            
        }

        // Render the form to create a new appointment
        public async Task<IActionResult> Create()
        {
            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();

            var model = new AppointmentViewModel
            {
                Date = DateTime.Now,
                //Animals = animals.Select(a => new SelectListItem
                //{
                //    Value = a.Id.ToString(),
                //    Text = a.Name
                //}).ToList(),
                Animals = animals.ToList(),
                //Veterinarians = veterinarians.Select(v => new SelectListItem
                //{
                //    Value = v.Id.ToString(),
                //    Text = $"{v.FirstName} {v.LastName}"
                //}).ToList(),
                Veterinarians = veterinarians.ToList(),
                //Customers = customers.Select(c => new SelectListItem
                //{
                //    Value = c.Id.ToString(),
                //    Text = $"{c.FirstName} {c.LastName}"
                //}).ToList()
                Customers = customers.ToList(),

                Rooms = rooms.ToList(),
            };

            return View(model);
        }

        // Handle form submission to create a new appointment
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    AppointmentDate = model.Date,
                    AnimalId = model.AnimalId,
                    VeterinarianId = model.VeterinarianId,
                    CustomerId = model.CustomerId,
                    RoomId = model.RoomId
                    // Populate other fields...
                };

                await _appointmentRepository.CreateAsync(appointment);
                return RedirectToAction(nameof(Index));
            }

            // Reload the lists if validation fails
            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();

            //model.Animals = animals.Select(a => new SelectListItem
            //{
            //    Value = a.Id.ToString(),
            //    Text = a.Name
            //}).ToList();
            model.Animals = animals.ToList();
            //model.Veterinarians = veterinarians.Select(v => new SelectListItem
            //{
            //    Value = v.Id.ToString(),
            //    Text = $"{v.FirstName} {v.LastName}"
            //}).ToList();

            model.Veterinarians = veterinarians.ToList();

            //model.Customers = customers.Select(c => new SelectListItem
            //{
            //    Value = c.Id.ToString(),
            //    Text = $"{c.FirstName} {c.LastName}"
            //}).ToList();
            model.Customers = customers.ToList();

            model.Rooms = rooms.ToList();

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();

            var model = new AppointmentViewModel
            {
                Id = appointment.Id,
                Date = appointment.AppointmentDate,
                AnimalId = appointment.AnimalId,
                VeterinarianId = appointment.VeterinarianId,
                CustomerId = appointment.CustomerId,
                RoomId = appointment.RoomId,

                //Animals = animals.Select(a => new SelectListItem
                //{
                //    Value = a.Id.ToString(),
                //    Text = a.Name
                //}).ToList(),
                Animals = animals.ToList(),
                //Veterinarians = veterinarians.Select(v => new SelectListItem
                //{
                //    Value = v.Id.ToString(),
                //    Text = $"{v.FirstName} {v.LastName}"
                //}).ToList(),
                Veterinarians = veterinarians.ToList(),

                //Customers = customers.Select(c => new SelectListItem
                //{
                //    Value = c.Id.ToString(),
                //    Text = $"{c.FirstName} {c.LastName}"
                //}).ToList()
                Customers = customers.ToList(),

                Rooms = rooms.ToList(),
            };

            return View(model);
        }

        // Handle form submission to update an existing appointment
        [HttpPost]
        public async Task<IActionResult> Edit(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = await _appointmentRepository.GetByIdAsync(model.Id);

                if (appointment == null)
                {
                    return NotFound();
                }

                appointment.AppointmentDate = model.Date;
                appointment.AnimalId = model.AnimalId;
                appointment.VeterinarianId = model.VeterinarianId;
                appointment.CustomerId = model.CustomerId;
                appointment.RoomId = model.RoomId;
                // Update other fields...

                await _appointmentRepository.UpdateAsync(appointment);
                return RedirectToAction(nameof(Index));
            }

            // Reload the lists if validation fails
            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();
            //model.Animals = animals.Select(a => new SelectListItem
            //{
            //    Value = a.Id.ToString(),
            //    Text = a.Name
            //}).ToList();
            model.Animals = animals.ToList();
            //model.Veterinarians = veterinarians.Select(v => new SelectListItem
            //{
            //    Value = v.Id.ToString(),
            //    Text = $"{v.FirstName} {v.LastName}"
            //}).ToList();

            model.Veterinarians = customers.ToList();

            //model.Customers = customers.Select(c => new SelectListItem
            //{
            //    Value = c.Id.ToString(),
            //    Text = $"{c.FirstName} {c.LastName}"
            //}).ToList();
            model.Customers = customers.ToList();

            model.Rooms = rooms.ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();

            var model = new AppointmentViewModel
            {
                Id = appointment.Id,
                Date = appointment.AppointmentDate,
                AnimalId = appointment.AnimalId,
                VeterinarianId = appointment.VeterinarianId,
                CustomerId = appointment.CustomerId,
                RoomId = appointment.RoomId,
              
                Animals = animals.ToList(),
              
                Veterinarians = veterinarians.ToList(),
             
                Customers = customers.ToList(),

                Rooms = rooms.ToList(),
            };
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();

            var model = new AppointmentViewModel
            {
                Id = appointment.Id,
                Date = appointment.AppointmentDate,
                AnimalId = appointment.AnimalId,
                VeterinarianId = appointment.VeterinarianId,
                CustomerId = appointment.CustomerId,
                RoomId = appointment.RoomId,

                Animals = animals.ToList(),

                Veterinarians = veterinarians.ToList(),

                Customers = customers.ToList(),

                Rooms = rooms.ToList(),
            };
            return View(model);
        }

        // Handle the deletion of an appointment
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            await _appointmentRepository.DeleteAsync(appointment);
            return RedirectToAction(nameof(Index));
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
            string subject = "Reminder: Appointment tomorrow!";

            if (appointments == null || !appointments.Any())
            {
                TempData["InfoMessage"] = "No appointments found for tomorrow.";
                return RedirectToAction("Index");
            }

            foreach (var appointment in appointments)
            {
                if (appointment.Customer == null || appointment.Veterinarian == null)
                {
                    return RedirectToAction("Index");
                }

                string body = $"Dear {appointment.Customer.FirstName},\nThis is a reminder that you have an appointment scheduled for tomorrow with Dr. {appointment.Veterinarian.FirstName}.";
                _emailHelper.SendEmail(appointment.Customer.Email, subject, body);
            }

            TempData["SuccessMessage"] = "Appointment reminders sent successfully.";
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

        public async Task<IActionResult> TestSendReminders()
        {
            await SendAppointmentRemindersAsync();
            TempData["SuccessMessage"] = "Appointment reminders sent successfully.";
            return RedirectToAction("Index"); 
        }
    }
}


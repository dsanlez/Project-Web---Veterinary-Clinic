using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                                      IEmailHelper emailHelper
                                      )
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
            var appointments = (await _appointmentRepository.GetAllWithUsersAsync()).OrderBy(ord => ord.AppointmentDate);
            return View(appointments);

        }

        // Render the form to create a new appointment
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            //var rooms = await _roomRepository.GetAllRoomsAsync();

            var model = new AppointmentViewModel
            {
                Date = DateTime.Now,

                Animals = animals.ToList(),

                Veterinarians = veterinarians.ToList(),

                //Customers = customers.ToList(),

                //Rooms = rooms.ToList(),
            };

            return View(model);
        }

        // Handle form submission to create a new appointment
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Date < DateTime.Now)
                {
                    TempData["AppointmentError"] = "The appointment cannot be scheduled at this time because the date in anterior to the actual date.";
                    
                    var animals = await _animalRepository.GetAllAnimalsAsync();
                    var veterinarians = await _userHelper.GetAllVeterinariansAsync();
                    //var customers = await _userHelper.GetAllCustomersAsync();
                    //var rooms = await _roomRepository.GetAllRoomsAsync();

                    model.Animals = animals.ToList();
                    model.Veterinarians = veterinarians.ToList();
                    //model.Customers = customers.ToList();
                    //model.Rooms = rooms.ToList();

                    return View(model);
                }
                var existingAppointments = await _appointmentRepository.GetAppointmentsByVeterinarianAsync(model.VeterinarianId);

                var startTime = model.Date;
                var endTime = startTime.AddMinutes(30);

                bool isConflict = existingAppointments.Any(a =>
                    (a.AppointmentDate < endTime && a.AppointmentDate.AddMinutes(30) > startTime));

                if (isConflict)
                {
                    TempData["AppointmentError"] = "The appointment cannot be scheduled at this time because there is already an appointment booked.";

                    model.Animals = (await _animalRepository.GetAllAnimalsAsync()).ToList();
                    model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
                    //model.Customers = (await _userHelper.GetAllCustomersAsync()).ToList();
                    //model.Rooms = (await _roomRepository.GetAllRoomsAsync()).ToList();
                    return View(model);
                }

                var veterinarian = await _userHelper.GetVeterinarianByIdAsync(model.VeterinarianId);

                model.RoomId = veterinarian.Room.Id;

                var appointment = new Appointment
                {
                    AppointmentDate = model.Date,
                    VeterinarianId = model.VeterinarianId,
                    RoomId = model.RoomId,
                    AnimalId = model.AnimalId
                };

                await _appointmentRepository.CreateAsync(appointment);
                TempData["SuccessAppointment"] = "Appointment Booked!";
                return RedirectToAction("Create");
            }

            model.Animals = (await _animalRepository.GetAllAnimalsAsync()).ToList();
            model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
            //model.Customers = (await _userHelper.GetAllCustomersAsync()).ToList();
            //model.Rooms = (await _roomRepository.GetAllRoomsAsync()).ToList();
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

                Animals = animals.ToList(),

                Veterinarians = veterinarians.ToList(),

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


                await _appointmentRepository.UpdateAsync(appointment);
                return RedirectToAction(nameof(Index));
            }

            // Reload the lists if validation fails
            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();

            model.Animals = animals.ToList();

            model.Veterinarians = customers.ToList();

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

            var customer = await _userHelper.GetUserByIdAsync(appointment.CustomerId); 
            var veterinarian = await _userHelper.GetUserByIdAsync(appointment.VeterinarianId);

            await _appointmentRepository.DeleteAsync(appointment);

            string subject = "Appointment Cancelled";
            string body = $"Dear {customer.FirstName},\n" +
                          $"Unfortunately, your appointment with Dr. {veterinarian.FirstName} has been cancelled.\n" +
                          "We will get in touch to reschedule the appointment.";

            var response = _emailHelper.SendEmail(customer.Email, subject, body); 

            if (!response.IsSuccess)
            {
                TempData["NotificationErrorMessage"] = "Error sending cancellation email: " + response.Message;
            }
            else
            {
                TempData["SuccessNotificationMessage"] = "Appointment cancelled and notification sent successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SendClosureNotification()
        {
            var customers = await _userHelper.GetCustomersAsync();
            var customerEmails = customers.Select(c => c.Email).ToList();

            string subject = "Closure Notification";
            string body = "Dear customer, the clinic will be closed due to unforeseen circumstances.";

            bool allEmailsSentSuccessfully = true;

            foreach (var email in customerEmails)
            {
                var response = _emailHelper.SendEmail(email, subject, body);
                if (!response.IsSuccess)
                {
                    allEmailsSentSuccessfully = false;
                    TempData["NotificationErrorMessage"] = "Error sending email to " + email + ": " + response.Message;
                }
            }
            if (allEmailsSentSuccessfully)
            {
                TempData["SuccessNotificationMessage"] = "Closure notifications sent successfully.";
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SendAppointmentReminderEmails()
        {
            // Get all appointments scheduled for tomorrow
            var appointments = await _appointmentRepository.GetAppointmentsForTomorrowAsync();

            if (!appointments.Any())
            {
                TempData["NotificationErrorMessage"] = "There are no appointments scheduled for tomorrow.";
                return RedirectToAction("Index");
            }

            // Send reminders for each appointment
            foreach (var appointment in appointments)
            {
                var customerEmail = appointment.Customer.Email;
                var subject = "Appointment Reminder";
                var body = $"Dear {appointment.Customer.FullName},\n\nThis is a reminder for your appointment tomorrow at {appointment.AppointmentDate:HH:mm}.";

                // Send email using the EmailHelper
                var response = _emailHelper.SendEmail(customerEmail, subject, body);

                if (!response.IsSuccess)
                {
                    TempData["NotificationErrorMessage"] = $"Error sending email to {customerEmail}: {response.Message}";
                }
            }
            TempData["SuccessNotificationMessage"] = "Reminder emails successfully sent for all appointments tomorrow.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> GetAvailableSlots(DateTime date, string veterinarianId)
        {
            // List to store available time slots
            List<string> availableSlots = new List<string>();

            if(veterinarianId == "0") 
            {
                return Json(availableSlots);
            }

            // Retrieve appointments for the specified veterinarian on the given date
            var appointments = await _appointmentRepository.GetAppointmentsByVeterinarianAndDate(veterinarianId, date);

            // Define the duration of each time slot and the clinic's operating hours
            TimeSpan slotDuration = TimeSpan.FromMinutes(30);// Duration of each appointment slot
            TimeSpan clinicOpeningTime = new TimeSpan(9, 0, 0); // 9:00 AM
            TimeSpan clinicClosingTime = new TimeSpan(17, 0, 0); // 5:00 PM
         
            // Create a list of all possible time slots within the clinic's operating hours
            for (TimeSpan time = clinicOpeningTime; time < clinicClosingTime; time += slotDuration)
            {
                // Check if the current time slot is occupied by any appointment
                bool isOccupied = appointments.Any(a => a.Date.TimeOfDay == time);

                // If the time slot is not occupied, add it to the list of available slots
                if (!isOccupied)
                {
                    // Format the time as hh:mm and add to the available slots list
                    availableSlots.Add(time.ToString(@"hh\:mm")); 
                }
            }
            // Return the list of available slots as a JSON response
            return Json(availableSlots);
        }


        //[Authorize(Roles = "Customer")]
        //public async Task<IActionResult> MyAppointments()
        //{
        //    // Obter o ID do cliente autenticado
        //    var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    // Obter as consultas que pertencem ao cliente autenticado
        //    var appointments = await _appointmentRepository.GetAppointmentsByCustomerAsync(user.Id);

        //    // Mapear as consultas para um ViewModel que será utilizado na view
        //    var model = appointments.Select(a => new AppointmentViewModel
        //    {
        //        Id = a.Id,
        //        Date = a.AppointmentDate,
        //        AnimalId = a.AnimalId,
        //        VeterinarianId = a.VeterinarianId,
        //        RoomId = a.RoomId,

        //    }).ToList();

        //    return View(model);
        //}

        [Authorize(Roles = "Customer")]  // Restringe o acesso apenas para o cliente
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            var appointments = await _appointmentRepository.GetAppointmentsByCustomerAsync(user.Id);

            return View(appointments);
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateForCustomer()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            var animals = await _animalRepository.GetAnimalsByOwnerAsync(user.Id); 
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var rooms = await _roomRepository.GetAllRoomsAsync();

            var model = new AppointmentViewModel
            {
                Date = DateTime.Now,
                Animals = animals.ToList(),
                Veterinarians = veterinarians.ToList(),
                Rooms = rooms.ToList()
            };

            return View(model);
        }
    }
}


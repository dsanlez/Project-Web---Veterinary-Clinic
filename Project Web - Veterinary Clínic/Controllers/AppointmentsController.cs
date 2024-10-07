using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Migrations;
using Project_Web___Veterinary_Clínic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

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
            var appointments = (await _appointmentRepository.GetAllWithUsersAsync())
               .Where(a => a.AppointmentDate >= DateTime.Now) 
               .OrderBy(ord => ord.AppointmentDate);

            return View(appointments);

        }
        // Render the form to create a new appointment
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var animals = await _animalRepository.GetAllAnimalsAsync();
            var veterinarians = await _userHelper.GetAllVeterinariansAsync();
            var customers = await _userHelper.GetAllCustomersAsync();

            var model = new AppointmentViewModel
            {
                Date = DateTime.Now,

                Animals = animals.ToList(),

                Veterinarians = veterinarians.ToList(),
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

                    model.Animals = animals.ToList();
                    model.Veterinarians = veterinarians.ToList();

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
                    return View(model);
                }
                var veterinarian = await _userHelper.GetVeterinarianByIdAsync(model.VeterinarianId);

                model.RoomId = veterinarian.Room.Id;

                var animal = await _animalRepository.GetByAnimalIdAsync(model.AnimalId);

                var appointment = new Appointment
                {
                    AppointmentDate = model.Date,
                    VeterinarianId = model.VeterinarianId,
                    RoomId = model.RoomId,
                    AnimalId = model.AnimalId,
                    Status = "Scheduled",
                    LastModified = DateTime.Now,
                    CustomerId = animal.Owner.Id,
                };

                await _appointmentRepository.CreateAsync(appointment);
                TempData["SuccessAppointment"] = "Appointment Booked!";
                return RedirectToAction("Create");
            }

            model.Animals = (await _animalRepository.GetAllAnimalsAsync()).ToList();
            model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
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
                if (model.Date < DateTime.Now)
                {
                    TempData["NotificationErrorMessage"] = "The appointment cannot be scheduled at this time because the date is anterior to the actual date.";

                    model.Animals = (await _animalRepository.GetAllAnimalsAsync()).ToList();
                    model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
                    model.Customers = (await _userHelper.GetAllCustomersAsync()).ToList();
                    model.Rooms = (await _roomRepository.GetAllRoomsAsync()).ToList();

                    return View(model);
                }
                var existingAppointments = await _appointmentRepository.GetAppointmentsByVeterinarianAsync(model.VeterinarianId);
                var startTime = model.Date;
                var endTime = startTime.AddMinutes(30);

                bool isConflict = existingAppointments.Any(a =>
                    (a.AppointmentDate < endTime && a.AppointmentDate.AddMinutes(30) > startTime));

                if (isConflict)
                {
                    TempData["NotificationErrorMessage"] = "The appointment cannot be scheduled at this time because there is already an appointment booked.";


                    model.Animals = (await _animalRepository.GetAllAnimalsAsync()).ToList();
                    model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
                    model.Customers = (await _userHelper.GetAllCustomersAsync()).ToList();
                    model.Rooms = (await _roomRepository.GetAllRoomsAsync()).ToList();

                    return View(model);
                }

                var appointment = await _appointmentRepository.GetByIdAsync(model.Id);

                if (appointment == null)
                {
                    return new NotFoundViewResult("AppointmentNotFound");
                }

                appointment.AppointmentDate = model.Date;
                appointment.AnimalId = model.AnimalId;
                appointment.VeterinarianId = model.VeterinarianId;
                appointment.CustomerId = model.CustomerId;
                appointment.RoomId = model.RoomId;
                appointment.Status = "Rescheduled";
                appointment.LastModified = DateTime.Now;

                await _appointmentRepository.UpdateAsync(appointment);
                TempData["SuccessNotificationMessage"] = "Appointment updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            model.Animals = (await _animalRepository.GetAllAnimalsAsync()).ToList();
            model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
            model.Customers = (await _userHelper.GetAllCustomersAsync()).ToList();
            model.Rooms = (await _roomRepository.GetAllRoomsAsync()).ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
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
                return new NotFoundViewResult("AppointmentNotFound");
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
                return new NotFoundViewResult("AppointmentNotFound");
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
                var customerEmail = appointment.Animal.Owner.Email;
                var subject = "Appointment Reminder";
                var body = $"Dear {appointment.Animal.Owner.FullName},\n\nThis is a reminder for your appointment tomorrow at {appointment.AppointmentDate:HH:mm}.";

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

            if (veterinarianId == "0")
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

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var appointments = await _appointmentRepository.GetAppointmentsByCustomerAsync(user.Id);

            var upcomingAppointments = appointments
                   .Where(a => a.AppointmentDate >= DateTime.Now)
                   .ToList();

            return View(upcomingAppointments);
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateForCustomer()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var animals = await _animalRepository.GetAnimalsByOwnerAsync(user.Id);

            var veterinarians = await _userHelper.GetAllVeterinariansAsync();

            var model = new AppointmentViewModel
            {
                Date = DateTime.Now,
                Animals = animals.ToList(),
                Veterinarians = veterinarians.ToList(),
            };
            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateForCustomer(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                // Verify if the date is not in the past
                if (model.Date < DateTime.Now)
                {
                    TempData["AppointmentError"] = "The appointment cannot be scheduled because the date is anterior to the current date.";

                    var animals = await _animalRepository.GetAnimalsByOwnerAsync(user.Id);
                    var veterinarians = await _userHelper.GetAllVeterinariansAsync();

                    model.Animals = animals.ToList();
                    model.Veterinarians = veterinarians.ToList();

                    return View(model);
                }

                var existingAppointments = await _appointmentRepository.GetAppointmentsByVeterinarianAsync(model.VeterinarianId);
                var startTime = model.Date;
                var endTime = startTime.AddMinutes(30);

                bool isConflict = existingAppointments.Any(a =>
                    (a.AppointmentDate < endTime && a.AppointmentDate.AddMinutes(30) > startTime));

                if (isConflict)
                {
                    TempData["AppointmentError"] = "The appointment cannot be scheduled because there is already an appointment booked.";

                    model.Animals = await _animalRepository.GetAnimalsByOwnerAsync(user.Id);
                    model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
                    return View(model);
                }

                var veterinarian = await _userHelper.GetVeterinarianByIdAsync(model.VeterinarianId);
                model.RoomId = veterinarian.Room.Id;

                var appointment = new Appointment
                {
                    AppointmentDate = model.Date,
                    AnimalId = model.AnimalId,
                    VeterinarianId = model.VeterinarianId,
                    CustomerId = user.Id,
                    RoomId = model.RoomId,
                    LastModified = DateTime.Now,
                    Status = "Scheduled",
                };
                await _appointmentRepository.CreateAsync(appointment);
                TempData["SuccessAppointment"] = "Appointment created successfully!";
                return RedirectToAction("CreateForCustomer");
            }
            model.Animals = await _animalRepository.GetAnimalsByOwnerAsync(this.User.Identity.Name);
            model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();
            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Reschedule(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

            var animals = await _animalRepository.GetAnimalsByOwnerAsync(user.Id);

            var veterinarians = await _userHelper.GetAllVeterinariansAsync();

            var model = new AppointmentViewModel
            {
                Id = appointment.Id,
                Date = appointment.AppointmentDate,
                AnimalId = appointment.AnimalId,
                VeterinarianId = appointment.VeterinarianId,
                RoomId = appointment.RoomId,
                Status = "Reschedule",

                Animals = animals.ToList(),
                Veterinarians = veterinarians.ToList()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Reschedule(int id, AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);

                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);

                if (appointment == null || appointment.CustomerId != user.Id)
                {
                    return new NotFoundViewResult("AppointmentNotFound");
                }

                if (model.Date < DateTime.Now)
                {
                    TempData["AppointmentError"] = "The appointment cannot be rescheduled because the date is anterior to the current date.";

                    var animals = await _animalRepository.GetAnimalsByOwnerAsync(user.Id);
                    var veterinarians = await _userHelper.GetAllVeterinariansAsync();

                    model.Animals = animals.ToList();
                    model.Veterinarians = veterinarians.ToList();

                    return View(model);
                }

                var existingAppointments = await _appointmentRepository.GetAppointmentsByVeterinarianAsync(model.VeterinarianId);

                var startTime = model.Date;
                var endTime = startTime.AddMinutes(30);

                bool isConflict = existingAppointments.Any(a =>
                    a.Id != id &&
                    (a.AppointmentDate < endTime && a.AppointmentDate.AddMinutes(30) > startTime));

                if (isConflict)
                {
                    TempData["AppointmentError"] = "The appointment cannot be rescheduled because there is already an appointment booked for the selected time.";

                    model.Animals = await _animalRepository.GetAnimalsByOwnerAsync(user.Id);
                    model.Veterinarians = (await _userHelper.GetAllVeterinariansAsync()).ToList();

                    return View(model);
                }

                appointment.AppointmentDate = model.Date;
                appointment.VeterinarianId = model.VeterinarianId;
                appointment.Status = "Rescheduled";
                appointment.LastModified = DateTime.Now;

                await _appointmentRepository.UpdateAsync(appointment);

                TempData["SuccessMessage"] = "Appointment rescheduled successfully!";
                return RedirectToAction("MyAppointments");
            }

            model.Animals = await _animalRepository.GetAnimalsByOwnerAsync(model.CustomerId);
            model.Veterinarians = await _userHelper.GetAllVeterinariansAsync();

            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null || appointment.CustomerId != (await _userHelper.GetUserByEmailAsync(User.Identity.Name)).Id)
            {
                return new NotFoundViewResult("AppointmentNotFound");
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

        [HttpPost]
        [ActionName("Cancel")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CancelConfirmed(int id)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(id);

            if (appointment == null || appointment.CustomerId != (await _userHelper.GetUserByEmailAsync(User.Identity.Name)).Id)
            {
                return new NotFoundViewResult("AppointmentNotFound");
            }

            appointment.LastModified = DateTime.Now;

            appointment.Status = "Cancelled";

            await _appointmentRepository.UpdateAsync(appointment);

            TempData["SuccessMessage"] = "Appointment cancelled successfully!";
            return RedirectToAction("MyAppointments");
        }

        public async Task<IActionResult> ClientAppointmentsHistory()
        {
            var customerId = (await _userHelper.GetUserByEmailAsync(User.Identity.Name)).Id;

            var appointments = await _appointmentRepository.GetAppointmentsByCustomerAsync(customerId);

            var pastAppointments = appointments
                .Where(a => a.AppointmentDate < DateTime.Now)
                .ToList();

            var model = new AppointmentHistoryViewModel
            {
                Appointments = pastAppointments,
                Animals = await _animalRepository.GetAllAnimalsByCustomerIdAsync(customerId) // Carregar todos os animais do cliente
            };
            return View(model);
        }

        public async Task<IActionResult> StaffAppointmentsHistory()
        {

            var appointments = await _appointmentRepository.GetAllWithUsersAsync();

            var pastAppointments = appointments
                .Where(a => a.AppointmentDate < DateTime.Now)
                .ToList();

            return View(pastAppointments);
        }

        public IActionResult AppointmentNotFound()
        {
            return View();
        }

    }
}


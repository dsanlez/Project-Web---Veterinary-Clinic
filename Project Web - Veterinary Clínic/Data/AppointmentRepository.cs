using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Migrations;
using Project_Web___Veterinary_Clínic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Data
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly DataContext _context;

        public AppointmentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllWithUsersAsync()
        {
            return await _context.Appointments
                .Include(a => a.Animal)
                .Include(a => a.Animal.Owner)
                .Include(a => a.Veterinarian)
                .Include(a => a.Customer)
                .Include(a => a.Room)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByVeterinarianAsync(string veterinarianId)
        {
            return await _context.Appointments
                .Where(a => a.VeterinarianId == veterinarianId).AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForTomorrowAsync()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var appointments = await _context.Appointments
                .Include(a => a.Animal.Owner)
                .Include(a => a.Customer)
                .Where(a => a.AppointmentDate.Date == tomorrow)
                .ToListAsync();

            return appointments;
        }

        public async Task<IEnumerable<AppointmentViewModel>> GetAppointmentsByVeterinarianAndDate(string veterinarianId, DateTime date)
        {

            var appointments = await _context.Appointments
                .Where(a => a.VeterinarianId == veterinarianId && a.AppointmentDate.Date == date.Date)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    Date = a.AppointmentDate,
                    AnimalId = a.AnimalId,

                    Animals = new List<SelectListItem>
                    {
                    new SelectListItem { Value = a.AnimalId.ToString(), Text = a.Animal.Name } // Get the animal name
                    },
                    VeterinarianId = a.VeterinarianId,

                    CustomerId = a.CustomerId,

                    Customers = new List<SelectListItem>
                    {
                    new SelectListItem { Value = a.CustomerId, Text = a.Customer.FullName } // Get the customer name
                    },
                    RoomId = a.RoomId,

                    Rooms = new List<SelectListItem>
                    {
                    new SelectListItem { Value = a.RoomId.ToString(), Text = a.Room.Name } // Get the room name
                    }
                })
                .ToListAsync();

            return appointments;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByCustomerAsync(string customerId)
        {
            return await _context.Appointments
            .Include(a => a.Animal)
            .Include(a => a.Veterinarian)
            .Include(a => a.Room)
            .Include(a => a.Customer)
            .Where(a => a.CustomerId == customerId)
            .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetRecentAlertsForStaffAsync()
        {
            var recentDate = DateTime.Today; 

            return await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Veterinarian)
                .Include(a => a.Animal)
                .Where(a => a.LastModified >= recentDate)
                .ToListAsync();
        }

        public IEnumerable<Appointment> GetAppointmentsByAnimalId(int animalId)
        {
            return _context.Appointments
                .Include(a => a.Animal)
                .Include(a => a.Customer)
                .Include(a => a.Veterinarian)
                .Where(a => a.AnimalId == animalId)
                .ToList();
        }
    }
}

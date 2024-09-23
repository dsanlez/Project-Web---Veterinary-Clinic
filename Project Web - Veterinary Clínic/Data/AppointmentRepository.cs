using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Migrations;
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
                .Include(a => a.Veterinarian)
                .Include(a => a.Customer)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByVeterinarianAsync(string veterinarianId)
        {
            return await _context.Appointments
                .Where(a => a.VeterinarianId == veterinarianId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsForTomorrowAsync()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            var appointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Date == tomorrow)
                .ToListAsync();

            return appointments;
        }
    }
}

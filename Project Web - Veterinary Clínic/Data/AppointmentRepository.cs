using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;
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

    }
}

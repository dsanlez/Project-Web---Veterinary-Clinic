using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;

namespace Project_Web___Veterinary_Clínic.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DbSet<Animal> Animals{ get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
    }
}

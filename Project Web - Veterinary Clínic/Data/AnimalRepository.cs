using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Linq;

namespace Project_Web___Veterinary_Clínic.Data
{
    public class AnimalRepository : GenericRepository<Animal>, IAnimalRepository
    {
        private readonly DataContext _context;

        public AnimalRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable GetAllWithUser()
        {
            return _context.Animals.Include(p => p.User);
        }
    }
}

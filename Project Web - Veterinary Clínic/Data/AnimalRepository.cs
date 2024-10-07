using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return _context.Animals.Include(a => a.Owner);
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAnimalsAsync()
        {
            var animals = await GetAll().Include(a => a.Owner).ToListAsync(); 

            var animalList = animals.Select(animal => new SelectListItem
            {
                Text = animal.Name + " - " + animal.Owner.FullName,  
                Value = animal.Id.ToString()
            }).ToList();
           
            return animalList;
        }

        public async Task<Animal> GetByAnimalIdAsync(int id)
        {
            return await _context.Animals
                .Include(a => a.Owner)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<SelectListItem>> GetAnimalsByOwnerAsync(string ownerId)
        {
            var animals = await _context.Animals
                                        .Where(a => a.OwnerId == ownerId)
                                        .ToListAsync();
            var animalSelectList = animals.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Name
            }).ToList();

            return animalSelectList;
        }

        public async Task<IEnumerable<Animal>> GetAllAnimalsByCustomerIdAsync(string customerId)
        {
            return await _context.Animals
                .Where(a => a.OwnerId == customerId)
                .ToListAsync();
        }
    }
}

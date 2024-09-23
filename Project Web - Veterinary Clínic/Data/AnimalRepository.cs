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
            return _context.Animals.Include(p => p.Dono);
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAnimalsAsync()
        {
            var animals = await GetAll().ToListAsync(); 

            var animalList = animals.Select(animal => new SelectListItem
            {
                Text = animal.Name,  
                Value = animal.Id.ToString()
            }).ToList();
           
            animalList.Insert(0, new SelectListItem
            {
                Text = "(Select an animal ...)",
                Value = "0"
            });

            return animalList;
        }

    }
}

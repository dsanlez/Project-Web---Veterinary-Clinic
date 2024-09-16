using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Data
{
    public interface IAnimalRepository : IGenericRepository<Animal>
    {
        public IQueryable GetAllWithUser();

        Task<IEnumerable<SelectListItem>> GetAllAnimalsAsync();
    }
}

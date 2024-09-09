using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Linq;

namespace Project_Web___Veterinary_Clínic.Data
{
    public interface IAnimalRepository : IGenericRepository<Animal>
    {
        public IQueryable GetAllWithUser();
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Data
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        Task<IEnumerable<SelectListItem>> GetAllRoomsAsync();
    }
}

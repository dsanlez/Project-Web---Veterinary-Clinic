using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Data
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        private readonly DataContext _context;
        public RoomRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllRoomsAsync()
        {
            var rooms = await GetAll().ToListAsync();

            var roomsList = rooms.Select(room => new SelectListItem
            {
                Text = room.Name,
                Value = room.Id.ToString()
            }).ToList();

            roomsList.Insert(0, new SelectListItem
            {
                Text = "(Select a room ...)",
                Value = "0"
            });

            return roomsList;
        }
    }
}

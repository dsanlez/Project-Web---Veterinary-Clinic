using Microsoft.AspNetCore.Mvc;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Helpers;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        // List all rooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _roomRepository.GetAll().ToListAsync();
            return View(rooms);
        }

        // Render the form to create a new room
        public IActionResult Create()
        {
            return View();
        }

        // Handle form submission to create a new room
        [HttpPost]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                await _roomRepository.CreateAsync(room);
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // Edit room - Render the form
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
            {
                return new NotFoundViewResult("RoomNotFound");
            }

            return View(room);
        }

        // Handle form submission to edit room
        [HttpPost]
        public async Task<IActionResult> Edit(Room room)
        {
            if (ModelState.IsValid)
            {
                await _roomRepository.UpdateAsync(room);
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        // Delete room - Render confirmation page
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
            {
                return new NotFoundViewResult("RoomNotFound");
            }

            return View(room);
        }

        // Handle form submission to delete room
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
            {
                return new NotFoundViewResult("RoomNotFound");
            }

            var hasAssociatedVeterinarians = await _roomRepository.HasAssociatedVeterinariansAsync(room.Id);

            if (hasAssociatedVeterinarians)
            {
                ModelState.AddModelError(string.Empty, "This room has associated veterinarians. Please reassign or delete the veterinarians before deleting the room.");
                return View(room);
            }

            await _roomRepository.DeleteAsync(room);
            return RedirectToAction(nameof(Index));
        }

        // Render the details of a room
        public async Task<IActionResult> Details(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);

            if (room == null)
            {
                return new NotFoundViewResult("RoomNotFound");
            }

            return View(room);
        }

        public IActionResult RoomNotFound()
        {
            return View();
        }

    }
}


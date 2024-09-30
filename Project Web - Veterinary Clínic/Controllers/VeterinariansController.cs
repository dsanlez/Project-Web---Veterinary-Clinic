using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Models;
using Syncfusion.EJ2.Spreadsheet;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class VeterinariansController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IRoomRepository _roomRepository;
        private readonly IImageHelper _imageHelper;

        public VeterinariansController(
            IUserHelper userHelper, 
            IEmailHelper emailHelper, 
            IRoomRepository roomRepository,
            IImageHelper imageHelper)
        {
            _userHelper = userHelper;
            _emailHelper = emailHelper;
            _roomRepository = roomRepository;
            _imageHelper = imageHelper;
        }
        public async Task<IActionResult> Index()
        {
            var veterinarians =  await _userHelper.GetVeterinariansAsync();
            return View(veterinarians);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new RegisterNewVeterinarianViewModel
            {
                Rooms = await _roomRepository.GetAllRoomsAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterNewVeterinarianViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                    var path = string.Empty;

                    if (model.AvatarFile != null && model.AvatarFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.AvatarFile, "users"); // Diretório para users
                    }

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        Specialty = model.Specialty,
                        RoomId = model.RoomId
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created");
                        return View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, "Veterinarian");

                    string token = await _userHelper.GeneratePasswordResetTokenAsync(user);

                    string resetLink = Url.Action("ResetPassword", "Account", new
                    {
                        user = user.Id,
                        email = user.Email,
                        token = token
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _emailHelper.SendEmail(model.Username, "Set your password",
                        $"<h1>Set your password</h1>Please set your password by clicking here: <br/><a href=\"{resetLink}\">Reset Password</a>");

                    if (response.IsSuccess)
                    {
                        TempData["SuccessMessage"] = "The veterinarian has been created successfully. Email was sent to set the password."; 
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError(string.Empty, "The email couldn't be sent");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered");
                }
            }
            model.Rooms = await _roomRepository.GetAllRoomsAsync();
            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            var veterinarian = await _userHelper.GetUserByIdAsync(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            return View(veterinarian);
        }

        // GET: Edit Veterinarian
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var veterinarian = await _userHelper.GetUserByIdAsync(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            var model = new EditNewVeterinarianViewModel
            {
                FirstName = veterinarian.FirstName,
                LastName = veterinarian.LastName,
                Username = veterinarian.Email,
                Address = veterinarian.Address,
                PhoneNumber = veterinarian.PhoneNumber,
                Specialty = veterinarian.Specialty,
                Rooms = await _roomRepository.GetAllRoomsAsync(),
                RoomId = veterinarian.RoomId
                
            };

            return View(model);
        }

        // POST: Edit Veterinarian
        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditNewVeterinarianViewModel model)
        {
            if (ModelState.IsValid)
            {
                var veterinarian = await _userHelper.GetUserByIdAsync(id);

                if (veterinarian == null)
                {
                    return NotFound();
                }

                if (model.RoomId == null)
                {
                    ModelState.AddModelError("RoomId", "Please select a room."); 
                    model.Rooms = await _roomRepository.GetAllRoomsAsync(); 
                    return View(model);
                }

                veterinarian.FirstName = model.FirstName;
                veterinarian.LastName = model.LastName;
                veterinarian.Email = model.Username;
                veterinarian.UserName = model.Username;
                veterinarian.Address = model.Address;
                veterinarian.PhoneNumber = model.PhoneNumber;
                veterinarian.Specialty = model.Specialty;
                veterinarian.RoomId = model.RoomId;

                var result = await _userHelper.UpdateUserAsync(veterinarian);

                if (result != IdentityResult.Success)
                {
                    ModelState.AddModelError(string.Empty, "The user couldn't be updated");
                    return View(model);
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Delete Veterinarian
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var veterinarian = await _userHelper.GetUserByIdAsync(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            return View(veterinarian);
        }

        // POST: Confirm Delete Veterinarian
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var veterinarian = await _userHelper.GetUserByIdAsync(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            var result = await _userHelper.DeleteUserAsync(veterinarian);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Veterinarian deleted successfully.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to delete Veterinarian.");
            return View(veterinarian);
        }

        public async Task<IActionResult> VeterinariansAndSpecialties()
        {
            var veterinarians = await _userHelper.GetVeterinariansAsync();
            return View(veterinarians);
        }
    }
}


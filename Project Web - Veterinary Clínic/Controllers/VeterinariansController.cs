using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Models;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class VeterinariansController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper _emailHelper;

        public VeterinariansController(IUserHelper userHelper, IEmailHelper emailHelper)
        {
            _userHelper = userHelper;
            _emailHelper = emailHelper;
        }
        public async Task<IActionResult> Index()
        {
            var veterinarians =  await _userHelper.GetVeterinariansAsync();
            return View(veterinarians);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created");
                        return View(model);
                    }


                    await _userHelper.AddUserToRoleAsync(user, "Veterinarian");

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _emailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email confirmation</h1>" +
                           $"To allow the user," +
                           $"please click on this link:</br></br><a href= \"{tokenLink}\">Confirm Email</a>");

                    if (response.IsSuccess)
                    {
                        TempData["SuccessMessage"] = "The veterinarian has been created successfully. The confirmation instructions have been sent.";
                        return RedirectToAction("Index");
                    }

                    ModelState.AddModelError(string.Empty, "The email couldn't be sent");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This email is already registered");
                }
            }
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

            var model = new EditUserViewModel
            {
                FirstName = veterinarian.FirstName,
                LastName = veterinarian.LastName,
                Username = veterinarian.Email,
                Address = veterinarian.Address,
                PhoneNumber = veterinarian.PhoneNumber
            };

            return View(model);
        }

        // POST: Edit Veterinarian
        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var veterinarian = await _userHelper.GetUserByIdAsync(id);

                if (veterinarian == null)
                {
                    return NotFound();
                }

                veterinarian.FirstName = model.FirstName;
                veterinarian.LastName = model.LastName;
                veterinarian.Email = model.Username;
                veterinarian.UserName = model.Username;
                veterinarian.Address = model.Address;
                veterinarian.PhoneNumber = model.PhoneNumber;

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
    }
}


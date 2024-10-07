using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class CustomersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEmailHelper _emailHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAnimalRepository _animalRepository;

        public CustomersController(IUserHelper userHelper,
            IEmailHelper emailHelper,
            IImageHelper imageHelper,
            IAppointmentRepository appointmentRepository,
            IAnimalRepository animalRepository)
        {
            _userHelper = userHelper;
            _emailHelper = emailHelper;
            _imageHelper = imageHelper;
            _appointmentRepository = appointmentRepository;
            _animalRepository = animalRepository;
        }
        public async Task<IActionResult> Index()
        {
            var customers = await _userHelper.GetCustomersAsync();
            return View(customers);
        }

        [HttpGet]
        [Authorize(Roles = "Veterinarian")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Create(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {

                    var existingPhoneNumberUser = await _userHelper.GetUserByPhoneNumberAsync(model.PhoneNumber);

                    if (existingPhoneNumberUser != null)
                    {
                        ModelState.AddModelError(string.Empty, "This phone number is already registered.");
                        return View(model);
                    }

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
                        Avatar = path
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user couldn't be created");
                        return View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, "Customer");

                    // Gerar token de redefinição de senha
                    string token = await _userHelper.GeneratePasswordResetTokenAsync(user);

                    string resetLink = Url.Action("ResetPassword", "Account", new
                    {
                        user = user.Id,
                        email = user.Email,
                        token = token
                    }, protocol: HttpContext.Request.Scheme);

                    // Enviar e-mail com o link de redefinição de senha
                    Response response = _emailHelper.SendEmail(model.Username, "Set your password",
                        $"<h1>Set your password</h1>Please set your password by clicking here: <br/><a href=\"{resetLink}\">Reset Password</a>");

                    if (response.IsSuccess)
                    {
                        TempData["SuccessMessage"] = "The customer has been created successfully. Email was sent to set the password.";
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


        [HttpGet]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var customer = await _userHelper.GetUserByIdAsync(id);
            if (customer == null || !await _userHelper.IsUserInRoleAsync(customer, "Customer"))
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var model = new RegisterNewUserViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Username = customer.Email,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var customer = await _userHelper.GetUserByIdAsync(id);
            if (customer == null || !await _userHelper.IsUserInRoleAsync(customer, "Customer"))
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var model = new EditUserViewModel
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Username = customer.Email,
                Address = customer.Address,
                PhoneNumber = customer.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Edit(string id, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = await _userHelper.GetUserByIdAsync(id);
                if (customer == null)
                {
                    return new NotFoundViewResult("CustomerNotFound");
                }

                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.Email = model.Username;
                customer.UserName = model.Username;
                customer.Address = model.Address;
                customer.PhoneNumber = model.PhoneNumber;

                var result = await _userHelper.UpdateUserAsync(customer);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Customer details updated successfully.";
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError(string.Empty, "Failed to update customer.");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var customer = await _userHelper.GetUserByIdAsync(id);

            if (customer == null )
            {
                return new NotFoundViewResult("CustomerNotFound");
            }
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var customer = await _userHelper.GetUserByIdAsync(id);

            if (customer == null)
            {
                return new NotFoundViewResult("CustomerNotFound");
            }

            var relatedAppointments = await _appointmentRepository.GetAppointmentsByCustomerAsync(customer.Id);

            if (relatedAppointments.Any())
            {

                ModelState.AddModelError(string.Empty, "This Customer has associated appointments. Please delete the appointments first.");

                return View(customer);
            }

            var relatedAnimals = await _animalRepository.GetAnimalsByOwnerAsync(customer.Id);

            if (relatedAnimals.Any())
            {
                ModelState.AddModelError(string.Empty, "This customer has registered animals. Please delete or reassign the animals before deleting the customer.");
                return View(customer);
            }

            var result = await _userHelper.DeleteUserAsync(customer);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Customer deleted successfully.";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Failed to delete customer.");
            return View(customer);
        }
        public IActionResult CustomerNotFound()
        {
            return View();
        }
    }
}

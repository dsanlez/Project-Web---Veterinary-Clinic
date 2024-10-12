using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        public readonly IUserHelper _userHelper;
        private readonly IAppointmentRepository _appointmentRepository;

        public AnimalsController(IAnimalRepository animalRepository,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IUserHelper userHelper,
            IAppointmentRepository appointmentRepository)
        {
            _animalRepository = animalRepository;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _userHelper = userHelper;
            _appointmentRepository = appointmentRepository;
        }

        // GET: Animals
        [Authorize(Roles = "Veterinarian")]
        public IActionResult Index()
        {
            return View(_animalRepository.GetAll().OrderBy(e => e.Name)
                .Include(a => a.Owner)
                .ToList());
        }

        // GET: Animals/Details/5
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _animalRepository.GetByAnimalIdAsync(id.Value);

            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            return View(animal);
        }

        // GET: Animals/Create
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Create()
        {
            var users = await _userHelper.GetAllCustomersAsync();
            var model = new AnimalViewModel
            {
                Owners = users.ToList(),
            };
            return View(model);
        }

        // POST: Animals/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Create(AnimalViewModel model)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "animals");

                }
                else
                {
                    ModelState.AddModelError("ImageFile", "The Image is required.");
                }

                var today = DateTime.Today;
                var maxBirthDate = today.AddYears(-20);

                if (model.BirthDate > today)
                {
                    ModelState.AddModelError("BirthDate", "Date of birth cannot be in the future.");
                }
                if (model.BirthDate < maxBirthDate)
                {
                    ModelState.AddModelError("BirthDate", "Date of birth must be within the last 20 years.");
                }

                if (ModelState.IsValid)
                {
                    var animal = _converterHelper.ToAnimal(model, path, true);
                    await _animalRepository.CreateAsync(animal);
                    return RedirectToAction(nameof(Index));
                }
            }
            var users = await _userHelper.GetAllCustomersAsync();
            model.Owners = users.ToList();

            return View(model);
        }

        // GET: Animals/Edit/5
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);

            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var owners = await _userHelper.GetAllCustomersAsync();

            var model = _converterHelper.ToAnimalViewModel(animal, owners);

            model.Owner = await _userHelper.GetUserByIdAsync(model.OwnerId);

            return View(model);
        }

        // POST: Animals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Edit(AnimalViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "animals");
                    }
                    var today = DateTime.Today;
                    var maxBirthDate = today.AddYears(-20);

                    if (model.BirthDate > today)
                    {
                        ModelState.AddModelError("BirthDate", "Date of birth cannot be in the future.");
                    }
                    if (model.BirthDate < maxBirthDate)
                    {
                        ModelState.AddModelError("BirthDate", "Date of birth must be within the last 20 years.");
                    }
                    
                    if (ModelState.IsValid)
                    {
                        var animal = _converterHelper.ToAnimal(model, path, false);
                        animal.Owner = await _userHelper.GetUserByIdAsync(model.OwnerId);

                        await _animalRepository.UpdateAsync(animal);
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _animalRepository.ExistsAsync(model.Id))
                    {
                        return new NotFoundViewResult("AnimalNotFound");
                    }
                    else
                    {
                        return new NotFoundViewResult("Error404");
                    }
                }
            }
            model.Owners = await _userHelper.GetAllCustomersAsync();
            return View(model);

        }

        // GET: Animals/Delete/5
        [Authorize(Roles = "Veterinarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);

            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Veterinarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var animal = await _animalRepository.GetByIdAsync(id);

            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            var relatedAppointments = _appointmentRepository.GetAppointmentsByAnimalId(animal.Id);

            if (relatedAppointments.Any())
            {

                ModelState.AddModelError(string.Empty, "This animal has associated appointments. Please delete the appointments first.");

                return View(animal);
            }

            await _animalRepository.DeleteAsync(animal);

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllAnimalsByOwner()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            return View(await _animalRepository.GetAllAnimalsByCustomerIdAsync(user.Id.ToString()));
        }
        public IActionResult AnimalNotFound()
        {
            return View();
        }
    }
}

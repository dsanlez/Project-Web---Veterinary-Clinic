﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using Project_Web___Veterinary_Clínic.Models;

namespace Project_Web___Veterinary_Clínic.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        public readonly IUserHelper _userHelper;

        public AnimalsController(IAnimalRepository animalRepository, 
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IUserHelper userHelper)
        {
            _animalRepository = animalRepository;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _userHelper = userHelper;
        }

        // GET: Animals
        public IActionResult Index()
        {
            return View( _animalRepository.GetAll().OrderBy(e => e.Name));
        }

        // GET: Animals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _animalRepository.GetByIdAsync(id.Value);

            if (animal == null)
            {
                return new NotFoundViewResult("AnimalNotFound");
            }

            return View(animal);
        }

        // GET: Animals/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Animals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnimalViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                var path = string.Empty;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(model.ImageFile, "animals");
                    
                }

                var animal = _converterHelper.ToAnimal(model, path, true);

                animal.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                await _animalRepository.CreateAsync(animal);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Animals/Edit/5
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

            var model = _converterHelper.ToAnimalViewModel(animal);
            return View(model);
        }
           
        

        // POST: Animals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

                    var animal = _converterHelper.ToAnimal(model, path, false);

                    animal.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

                    await _animalRepository.UpdateAsync(animal);

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
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Animals/Delete/5
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _animalRepository.GetByIdAsync(id);
            await _animalRepository.DeleteAsync(product);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult AnimalNotFound()
        {
            return View();
        }
    }
}

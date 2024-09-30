using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Models;
using System.Collections.Generic;

namespace Project_Web___Veterinary_Clínic.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Animal ToAnimal(AnimalViewModel model, string path, bool isNew)
        {
            return new Animal
            {
                Id = isNew ? 0 : model.Id,
                Name = model.Name,
                ImageUrl = path,
                OwnerId = model.OwnerId,
                BirthDate = model.BirthDate,
                Species = model.Species,
            };
        }

        public AnimalViewModel ToAnimalViewModel(Animal animal, IEnumerable<SelectListItem> owners)
        {
            return new AnimalViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                ImageUrl = animal.ImageUrl,
                OwnerId = animal.OwnerId,
                BirthDate = animal.BirthDate,
                Species = animal.Species,
                Owners = owners
            };
        }
    }
}

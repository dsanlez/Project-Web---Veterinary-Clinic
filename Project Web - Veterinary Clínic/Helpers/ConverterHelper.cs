using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Models;

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
                User = model.User,
                BirthDate = model.BirthDate,
                Species = model.Species,
            };
        }

        public AnimalViewModel ToAnimalViewModel(Animal animal)
        {
            return new AnimalViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                ImageUrl = animal.ImageUrl,
                User = animal.User,
                BirthDate = animal.BirthDate,
                Species = animal.Species,
            };
        }
    }
}

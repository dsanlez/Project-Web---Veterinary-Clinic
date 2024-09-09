using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Models;

namespace Project_Web___Veterinary_Clínic.Helpers
{
    public interface IConverterHelper
    {
        Animal ToAnimal(AnimalViewModel model, string path, bool isNew);

        AnimalViewModel ToAnimalViewModel(Animal animal);
    }
}

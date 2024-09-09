using Microsoft.AspNetCore.Http;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class AnimalViewModel : Animal
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}

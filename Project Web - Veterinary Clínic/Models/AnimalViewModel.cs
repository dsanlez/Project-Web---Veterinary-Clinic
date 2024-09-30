using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Models
{
    public class AnimalViewModel : Animal
    {
        [Required]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        //[Required]
        //[Display(Name = "Owner")]
        //public string OwnerId { get; set; }

        public IEnumerable<SelectListItem> Owners { get; set; } 
    }
}

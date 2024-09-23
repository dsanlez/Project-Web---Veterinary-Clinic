using System;
using System.ComponentModel.DataAnnotations;

namespace Project_Web___Veterinary_Clínic.Data.Entities
{
    public class Animal : IEntity
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Species { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required]
        public User Dono { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://localhost:44363{ImageUrl.Substring(1)}";
            }
        }

    }
}

using Microsoft.AspNetCore.Identity;

namespace Project_Web___Veterinary_Clínic.Data.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

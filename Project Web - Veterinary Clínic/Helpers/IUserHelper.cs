using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<IdentityResult> DeleteUserAsync(User user);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);

        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<User> GetUserByIdAsync(string userId);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<IEnumerable<SelectListItem>> GetRolesSelectListAsync();

        Task<IEnumerable<SelectListItem>> GetAllVeterinariansAsync();
        Task<IEnumerable<SelectListItem>> GetAllCustomersAsync();

        Task<IEnumerable<User>> GetCustomersAsync();

        Task<IEnumerable<User>> GetVeterinariansAsync();

        Task<User> GetVeterinarianByIdAsync(string veterinarianId);
    }
}











using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Web___Veterinary_Clínic.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserHelper(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> DeleteUserAsync(User user)
        {
            return await _userManager.DeleteAsync(user);
        }


        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Username, model.Password, model.RememberMe, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName
                });
            }
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }

        public async Task<IEnumerable<SelectListItem>> GetRolesSelectListAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

        }

        public async Task<IEnumerable<SelectListItem>> GetAllVeterinariansAsync()
        {
            var roleName = "Veterinarian";
           
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            var veterinarianList = usersInRole.Select(user => new SelectListItem
            {
                Text = user.FullName,
                Value = user.Id.ToString()
            }).ToList();

            return veterinarianList;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllCustomersAsync()
        {
            var roleName = "Customer";

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            var customerList = usersInRole.Select(user => new SelectListItem
            {
                Text = user.FullName + " - " + user.Email,
                Value = user.Id.ToString()
            }).ToList();

            return customerList;
        }

        public async Task<IEnumerable<User>> GetCustomersAsync()
        {
            var roleName = "Customer";
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            return usersInRole;
        }

        public async Task<IEnumerable<User>> GetVeterinariansAsync()
        {
            var roleName = "Veterinarian";
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);

            var veterinarians = await _userManager.Users
            .Include(u => u.Room) 
            .Where(u => usersInRole.Contains(u)) 
            .ToListAsync();

            return veterinarians;
        }

        public async Task<User> GetVeterinarianByIdAsync(string veterinarianId)
        {
             return await _userManager.Users
            .Include(u => u.Room)
            .FirstOrDefaultAsync(u => u.Id == veterinarianId);
        }

        public async Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }



    }
}

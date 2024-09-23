using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_Web___Veterinary_Clínic.Data.Entities;
using Project_Web___Veterinary_Clínic.Helpers;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Project_Web___Veterinary_Clínic.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private Random _random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;

            _random = new Random();
        }

        public IUserHelper UserManager { get; }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Veterinarian");
            await _userHelper.CheckRoleAsync("Customer");
            //await _userHelper.CheckRoleAsync("Employee");

            var user = await _userHelper.GetUserByEmailAsync("diogosdl25@hotmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Diogo",
                    LastName = "Sanlez",
                    Email = "diogosdl25@hotmail.com",
                    UserName = "diogosdl25@hotmail.com",
                    PhoneNumber = "1234567890",
                };

                var result = await _userHelper.AddUserAsync(user, "123456");

                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Couldn't create the user in seeder");
                }

                await _userHelper.AddUserToRoleAsync(user, "Admin");

                var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

                await _userHelper.ConfirmEmailAsync(user, token);
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            if (!_context.Animals.Any())
            {
                AddAnimal("Pirucas", "Cat", user);
                AddAnimal("Bobby", "Dog", user);
                AddAnimal("Cajó", "Dog", user);
                AddAnimal("Marilú", "Dog", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddAnimal(string name, string species, User user)
        {
            int year = _random.Next(2020, DateTime.Now.Year);
            int month = _random.Next(1, 13); 
            int day = _random.Next(1, DateTime.DaysInMonth(year, month) + 1); 

            _context.Animals.Add(new Animal
            {
                Name = name,
                Species = species,
                BirthDate = new DateTime(year, month, day),
                Dono = user
            }) ;
        }
    }
}

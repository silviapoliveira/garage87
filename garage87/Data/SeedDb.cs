using garage87.Data.Entities;
using garage87.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Data
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

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("Customer");

            if (!_context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Lisboa" });
                cities.Add(new City { Name = "Porto" });
                cities.Add(new City { Name = "Faro" });

                _context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Portugal"
                });

                await _context.SaveChangesAsync();
            }

            var user = await _userHelper.GetUserByEmailAsync("silvia.cet87@gmail.com");

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Sílvia",
                    LastName = "Oliveira",
                    Email = "silvia.cet87@gmail.com",
                    UserName = "silvia.cet87@gmail.com",
                    PhoneNumber = "912345678",
                    Address = "Travessa Combatentes do Ultramar 80",
                    CityId = _context.Countries.FirstOrDefault().Cities.FirstOrDefault().Id,
                    City = _context.Countries.FirstOrDefault().Cities.FirstOrDefault()
                };

                var result = await _userHelper.AddUserAsync(user, "123456");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder.");
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

            if (!_context.Customers.Any())
            {
                AddCustomer("Miguel", "Petiz", "miguelpetiz@yopmail.com", "123456789", user);
                AddCustomer("Alice", "Castro", "alicecastro@yopmail.com", "987654321", user);
                AddCustomer("Rui", "Fonseca", "ruifonseca@yopmail.com", "192837465", user);
                AddCustomer("Paula", "Dias", "pauladias@yopmail.com", "918273645", user);
                await _context.SaveChangesAsync();
            }

            if (!_context.Employees.Any())
            {
                AddEmployee("Pedro", "Oliveira", "Mecânico", user);
                AddEmployee("Artur", "Couto", "Mecânico", user);
                AddEmployee("Tânia", "Justo", "Recepcionista", user);
                await _context.SaveChangesAsync();
            }
        }

        private void AddCustomer(string firstName, string lastName, string email, string vatNumber, User user)
        {
            _context.Customers.Add(new Entities.Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                VatNumber = vatNumber,
                User = user
            });
        }

        private void AddEmployee(string firstName, string lastName, string function, User user)
        {
            _context.Employees.Add(new Entities.Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Function = function,
                User = user
            });
        }
    }
}

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
            await _userHelper.CheckRoleAsync("Employee");
            await _userHelper.CheckRoleAsync("Mechanic");


            if (!_context.Countries.Any())
            {
                var cities = new List<City>();
                cities.Add(new City { Name = "Lisboa" });
                cities.Add(new City { Name = "Porto" });
                cities.Add(new City { Name = "Faro" });

                _context.Countries.Add(new Country
                {
                    Cities = cities,
                    Name = "Portugal",
                    CountryCode = "PT"
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
                    Address = "Rua Ada Lovelace, 7",
                    EmailConfirmed = true,
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
                // Example customer data
                var customer = new Customer
                {
                    FirstName = "Miguel",
                    LastName = "Petiz",
                    Email = "miguelpetiz@yopmail.com",
                    PhoneNumber = "912345678",
                    VatNumber = "912345678",
                    CityId = 1,
                    Address = "Rua da Alegria",
                    ZipCode = "2176",
                };

                // Create a user associated with this customer
                var Customeruser = new User
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    UserName = customer.Email,
                    PhoneNumber = customer.PhoneNumber,
                    CityId = customer.CityId,
                    Address = customer.Address,
                    EmailConfirmed = true,

                };

                var result = await _userHelper.AddUserAsync(Customeruser, "123456");

                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(Customeruser, "Customer");
                    customer.UserId = Customeruser.Id;
                    await _context.Customers.AddAsync(customer);
                    await _context.SaveChangesAsync();
                }
            }

            if (!_context.Employees.Any())
            {
                // Example employee data
                var employee = new Employee
                {
                    FirstName = "Mariana",
                    LastName = "Magalhães",
                    Salary = 1000,
                    VatNumber = "123456789",
                    Function = (int)Enums.EmployeeFunctionEnum.Receptionist,
                };

                // Create a user associated with this employee
                var employeeUser = new User
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = "marianamagalhaes@yopmail.com",
                    UserName = "marianamagalhaes@yopmail.com",
                    PhoneNumber = "123456789",
                    CityId = 1,
                    Address = "Rua das Flores, 17",
                    EmailConfirmed = true,
                };

                var result = await _userHelper.AddUserAsync(employeeUser, "123456");

                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRoleAsync(employeeUser, "Employee");
                    employee.UserId = employeeUser.Id;
                    await _context.Employees.AddAsync(employee);
                }


                // Save all changes to the database
                await _context.SaveChangesAsync();
            }


            if (!_context.Services.Any())
            {
                var services = new List<Service>
                {
                    new Service { Name = "Oil and filter change", Price = 100, ImageUrl = "~/images/services/oilandfilterchange.jpg" ,Description="The oil lubricates the engine, minimizing friction between parts and preventing overheating. Regular oil and filter changes are essential for engine efficiency. This service typically includes draining the old oil, replacing the oil filter, and refilling with new oil to ensure smooth engine operation."},
                    new Service { Name = "Brake maintenance", Price = 150, ImageUrl = "~/images/services/brakemaintenance.jpg",Description="Involves checking and replacing components like pads, discs, and brake fluid to ensure vehicle safety. This service ensures that the braking system is functioning correctly, providing reliable stopping power and preventing potential accidents caused by brake failure." },
                    new Service { Name = "Alignment and balancing", Price = 60,ImageUrl = "~/images/services/alignmentandbalancing.jpg",Description="Ensures that the wheels are parallel to each other and perpendicular to the ground, providing even tire wear and a smooth ride. Proper alignment and balancing improve handling, fuel efficiency, and extend the lifespan of the tires." },
                    new Service { Name = "Tire replacement", Price = 15,  ImageUrl = "~/images/services/tirereplacement.jpg",Description="Worn or damaged tires compromise the vehicle’s grip and braking ability. This service includes removing old tires, inspecting the condition of the wheels, and installing new tires to ensure optimal performance and safety on the road." },
                    new Service { Name = "Engine repairs", Price = 800,  ImageUrl = "~/images/services/enginerepair.jpg",Description="From simple fixes to complete overhauls, ensuring optimal engine performance. This can include diagnosing and repairing issues such as faulty sensors, worn-out components, and other mechanical problems that affect the engine’s efficiency and reliability." },
                    new Service { Name = "Suspension and steering system", Price = 350,  ImageUrl = "~/images/services/suspensionandsteeringsystem.jpg",Description="Ensures driving comfort and proper tire-to-road contact. This service involves inspecting and repairing components like shock absorbers, struts, and steering mechanisms to maintain vehicle stability and handling." },
                    new Service { Name = "Electrical and electronic services", Price = 200,  ImageUrl = "~/images/services/electricalandelectronicservices.jpg",Description="Troubleshoot issues in components like batteries, alternators, and electronic injection systems. This service ensures that the vehicle’s electrical systems are functioning correctly, preventing issues like starting problems, battery drain, and malfunctioning electronic components." },
                    new Service { Name = "Inspections and preventive maintenance", Price = 250, ImageUrl = "~/images/services/inspectionsandpreventivemaintenance.jpg",Description="Includes periodic checks to prevent future problems and ensure the vehicle’s proper functioning. This service typically covers a comprehensive inspection of various systems and components, identifying potential issues before they become major problems." },
                    new Service { Name = "Air conditioning service", Price = 275, ImageUrl = "~/images/services/airconditioning.jpg",Description="Maintenance and repair of the vehicle’s air conditioning system. This includes checking refrigerant levels, inspecting for leaks, and ensuring that the system is cooling effectively to provide comfort during hot weather." },
                    new Service { Name = "Fluid and lubricant changes", Price = 100, ImageUrl = "~/images/services/fluidandlubricantchanges.jpg",Description="Includes changing various fluids, such as transmission, power steering, and brake fluids. Regular fluid changes are essential to maintain the performance and longevity of the vehicle’s systems, preventing wear and tear caused by contaminated or degraded fluids." }
                };

                await _context.Services.AddRangeAsync(services);
                await _context.SaveChangesAsync();
            }
        }
    }
}

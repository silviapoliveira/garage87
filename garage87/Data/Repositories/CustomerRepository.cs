using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Data.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly DataContext _context;

        public CustomerRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddVehicleAsync(VehicleViewModel model)
        {
            var customer = await this.GetCustomerWithVehiclesAsync(model.CustomerId);
            if (customer == null)
            {
                return;
            }

            customer.Vehicles.Add(new Vehicle
            {
                Type = model.Type,
                BrandId = model.BrandId,
                ModelId = model.ModelId,
                Registration = model.Registration,
                Year = model.Year,
                Month = model.Month
            });
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteVehicleAsync(Vehicle vehicle)
        {
            var customer = await _context.Customers
                .Where(c => c.Vehicles.Any(ci => ci.Id == vehicle.Id))
                .FirstOrDefaultAsync();
            if (customer == null)
            {
                return 0;
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return customer.Id;
        }

        public IQueryable GetCustomersWithVehicles()
        {
            return _context.Customers
                .Include(c => c.Vehicles)
                .OrderBy(c => c.FirstName);
        }

        public IQueryable<string> GetCustomerEmails()
        {
            var customerRoleId = _context.Roles
                .Where(r => r.Name == "Customer")
                .Select(r => r.Id)
                .FirstOrDefault();

            return _context.UserRoles
                .Where(ur => ur.RoleId == customerRoleId)
                .Select(ur => ur.UserId)
                .Join(_context.Users,
                      userId => userId,
                      user => user.Id,
                      (userId, user) => user.Email)
                .Distinct();
        }

        public IQueryable<string> GetReminderEmails()
        {
            var userIds = _context.VehicleAssignment
                .Where(x => x.TaskDate.Date == DateTime.Today.AddDays(1))
                .Select(x => x.Vehicle.Customer.UserId)
                .Distinct();

            return _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => u.Email)
                .Distinct();
        }

        public async Task<Customer> GetCustomerWithVehiclesAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Vehicles)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> UpdateVehicleAsync(Vehicle vehicle)
        {
            var customer = await _context.Customers
                .Where(c => c.Vehicles.Any(ci => ci.Id == vehicle.Id)).FirstOrDefaultAsync();
            if (customer == null)
            {
                return 0;
            }

            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return customer.Id;
        }

        public async Task<Vehicle> GetVehicleAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<Customer> GetCustomerAsync(Vehicle vehicle)
        {
            return await _context.Customers
                .Where(c => c.Vehicles.Any(ci => ci.Id == vehicle.Id))
                .FirstOrDefaultAsync();
        }

        public IEnumerable<SelectListItem> GetComboCustomers()
        {
            var list = _context.Customers.Select(c => new SelectListItem
            {
                Text = c.FullName,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a customer...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboVehicles(int customerId)
        {
            var customer = _context.Customers.Find(customerId);
            var list = new List<SelectListItem>();
            if (customer != null)
            {
                list = _context.Vehicles.Select(c => new SelectListItem
                {
                    Text = c.Registration,
                    Value = c.Id.ToString()

                }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a vehicle...)",
                    Value = "0"
                });
            }

            return list;
        }
    }
}

using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Data.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        //public async Task AddServiceAsync(ServiceViewModel model)
        //{
        //    var employee = await this.GetEmployeeWithServicesAsync(model.EmployeeId);
        //    if (employee == null)
        //    {
        //        return;
        //    }

        //    employee.Services.Add(new Service { Name = model.Name });
        //    _context.Employees.Update(employee);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task<int> DeleteServiceAsync(Service service)
        //{
        //    var employee = await _context.Employees
        //        .Where(c => c.Services.Any(ci => ci.Id == service.Id))
        //        .FirstOrDefaultAsync();
        //    if (employee == null)
        //    {
        //        return 0;
        //    }

        //    _context.Services.Remove(service);
        //    await _context.SaveChangesAsync();
        //    return employee.Id;
        //}

        //public IQueryable GetEmployeesWithServices()
        //{
        //    return _context.Employees
        //        .Include(c => c.Services)
        //        .OrderBy(c => c.FirstName);
        //}

        //public async Task<Employee> GetEmployeeWithServicesAsync(int id)
        //{
        //    return await _context.Employees
        //        .Include(c => c.Services)
        //        .Where(c => c.Id == id)
        //        .FirstOrDefaultAsync();
        //}

        //public async Task<int> UpdateServiceAsync(Service service)
        //{
        //    var employee = await _context.Employees
        //        .Where(c => c.Services.Any(ci => ci.Id == service.Id)).FirstOrDefaultAsync();
        //    if (employee == null)
        //    {
        //        return 0;
        //    }

        //    _context.Services.Update(service);
        //    await _context.SaveChangesAsync();
        //    return employee.Id;
        //}

        //public async Task<Service> GetServiceAsync(int id)
        //{
        //    return await _context.Services.FindAsync(id);
        //}

        //public async Task<Employee> GetEmployeeAsync(Service service)
        //{
        //    return await _context.Employees
        //        .Where(c => c.Services.Any(ci => ci.Id == service.Id))
        //        .FirstOrDefaultAsync();
        //}

        public IEnumerable<SelectListItem> GetComboEmployees()
        {
            var list = _context.Employees.Select(c => new SelectListItem
            {
                Text = c.NameFunction,
                Value = c.Id.ToString()

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a employee...)",
                Value = "0"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboServices(int employeeId)
        {
            var employee = _context.Employees.Find(employeeId);
            var list = new List<SelectListItem>();
            if (employee != null)
            {
                list = _context.Services.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()

                }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a service...)",
                    Value = "0"
                });
            }

            return list;
        }
    }
}

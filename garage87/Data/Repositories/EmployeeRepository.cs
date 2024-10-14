using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace garage87.Data.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

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

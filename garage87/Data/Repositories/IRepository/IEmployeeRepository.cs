using garage87.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace garage87.Data.Repositories.IRepository
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        IEnumerable<SelectListItem> GetComboEmployees();

        IEnumerable<SelectListItem> GetComboServices(int employeeId);
    }
}

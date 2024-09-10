using garage87.Data.Entities;
using garage87.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Data.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        IQueryable GetEmployeesWithServices();


        Task<Employee> GetEmployeeWithServicesAsync(int id);


        Task<Service> GetServiceAsync(int id);


        Task AddServiceAsync(ServiceViewModel model);


        Task<int> UpdateServiceAsync(Service service);


        Task<int> DeleteServiceAsync(Service service);

        IEnumerable<SelectListItem> GetComboEmployees();

        IEnumerable<SelectListItem> GetComboServices(int employeeId);


        Task<Employee> GetEmployeeAsync(Service service);
    }
}

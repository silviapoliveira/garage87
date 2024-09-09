using garage87.Data.Entities;
using garage87.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Data.Repositories
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        IQueryable GetCustomersWithVehicles();


        Task<Customer> GetCustomerWithVehiclesAsync(int id);


        Task<Vehicle> GetVehicleAsync(int id);


        Task AddVehicleAsync(VehicleViewModel model);


        Task<int> UpdateVehicleAsync(Vehicle vehicle);


        Task<int> DeleteVehicleAsync(Vehicle vehicle);

        IEnumerable<SelectListItem> GetComboCustomers();

        IEnumerable<SelectListItem> GetComboVehicles(int customerId);


        Task<Customer> GetCustomerAsync(Vehicle vehicle);
    }
}

using garage87.Data.Entities;
using garage87.Models;

namespace garage87.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public Customer ToCustomer(CustomerViewModel model, string path, bool isNew)
        {
            return new Customer
            {
                Id = isNew ? 0 : model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageUrl = path,
                Address = model.Address,
                ZipCode = model.ZipCode,
                CityId = model.CityId,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                VatNumber = model.VatNumber
            };
        }

        public CustomerViewModel ToCustomerViewModel(Customer customer)
        {
            return new CustomerViewModel
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                ImageUrl = customer.ImageUrl,
                Address = customer.Address,
                ZipCode = customer.ZipCode,
                CityId = customer.CityId,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                VatNumber = customer.VatNumber
            };
        }

        public Employee ToEmployee(EmployeeViewModel model, string path, bool isNew)
        {
            return new Employee
            {
                Id = isNew ? 0 : model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageUrl = path,
                Function = model.Function,
                Salary = model.Salary,
                VatNumber = model.VatNumber
            };
        }

        public EmployeeViewModel ToEmployeeViewModel(Employee employee)
        {
            return new EmployeeViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                ImageUrl = employee.ImageUrl,
                Function = employee.Function,
                Salary = employee.Salary,
                VatNumber = employee.VatNumber,
                CityId = employee.User.CityId,
                Email = employee.User.Email,
                PhoneNo = employee.User.PhoneNumber,
            };
        }
    }
}

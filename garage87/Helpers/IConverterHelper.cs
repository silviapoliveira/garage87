using garage87.Data.Entities;
using garage87.Models;

namespace garage87.Helpers
{
    public interface IConverterHelper
    {
        Customer ToCustomer(CustomerViewModel model, string path, bool isNew);

        CustomerViewModel ToCustomerViewModel(Customer customer);

        Employee ToEmployee(EmployeeViewModel model, string path, bool isNew);

        EmployeeViewModel ToEmployeeViewModel(Employee employee);
    }
}

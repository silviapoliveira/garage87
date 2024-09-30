using garage87.Data.Entities;
using System.Collections.Generic;

namespace garage87.Models
{
    public class HomeVM
    {
        public IEnumerable<Employee> Mechanics { get; set; }


        public IEnumerable<Service> Services { get; set; }
    }
}

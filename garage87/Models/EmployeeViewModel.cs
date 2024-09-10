using garage87.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace garage87.Models
{
    public class EmployeeViewModel : Employee
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}

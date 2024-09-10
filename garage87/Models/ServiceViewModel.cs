using System.ComponentModel.DataAnnotations;

namespace garage87.Models
{
    public class ServiceViewModel
    {
        public int EmployeeId { get; set; }


        public int ServiceId { get; set; }


        [Required]
        [Display(Name = "Service")]
        [MaxLength(200, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace garage87.Data.Entities
{
    public class Employee : IEntity
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "")]
        public string ImageUrl { get; set; }


        [Required]
        [MaxLength(200, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        public string Function { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Salary { get; set; }


        [Required]
        [MaxLength(15, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "VAT Number")]
        public string VatNumber { get; set; }


        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";


        [Display(Name = "Name and Function")]
        public string NameFunction => $"{FirstName} {LastName} - {Function}";


        // List of services associated to the employee
        public ICollection<Service> Services { get; set; }


        [Display(Name = "Number of Services")]
        public int NumberServices => Services == null ? 0 : Services.Count;

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return "https://localhost:44331/images/employees/noimage.jpg";
                }

                return $"https://localhost:44331{ImageUrl.Substring(1)}";
            }
        }
    }
}

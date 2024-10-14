using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int? Function { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Salary { get; set; }


        [MaxLength(15, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "VAT Number")]
        public string VatNumber { get; set; }


        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";


        [Display(Name = "Name and Function")]
        public string NameFunction => $"{FirstName} {LastName} - {Function}";


        public string AddedBy { get; set; }


        public string UserId { get; set; }


        [Display(Name = "Speciality")]
        public int? SpecialityId { get; set; }


        [ForeignKey("UserId")]
        public virtual User User { get; set; }


        [ForeignKey("SpecialityId")]
        public virtual Specialities Specialities { get; set; }


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


        public ICollection<VehicleAssignment> VehicleAssignment { get; set; }
    }
}

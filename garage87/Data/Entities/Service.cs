using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace garage87.Data.Entities
{
    public class Service : IEntity
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "Service")]
        [MaxLength(200, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Vat { get; set; }


        [Required]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}

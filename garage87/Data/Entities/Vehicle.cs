using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Data.Entities
{
    public class Vehicle : IEntity
    {
        public int Id { get; set; }


        [Required]
        public string Type { get; set; }


        [Required]
        [Display(Name = "Brand")]
        public int? BrandId { get; set; }


        [Required]
        [Display(Name = "Model")]
        public int? ModelId { get; set; }


        [Required]
        [Display(Name = "Registration")]
        public string Registration { get; set; }


        [Required]
        public int Year { get; set; }


        [Required]
        public int Month { get; set; }


        [Required]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }


        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }


        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }


        [ForeignKey("ModelId")]
        public virtual Model Model { get; set; }


        public ICollection<VehicleAssignment> VehicleAssignment { get; set; }
    }
}

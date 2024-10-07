using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace garage87.Data.Entities
{
    public class Vehicle : IEntity
    {
        public int Id { get; set; }


        [Required]
        public string Type { get; set; }


        [Required]
        public string Brand { get; set; }


        [Required]
        public string Model { get; set; }


        [Required]
        public string Registration { get; set; }


        [Required]
        public int Year { get; set; }


        [Required]
        public int Month { get; set; }


        [Required]
        public int CustomerId { get; set; }


        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }


        [Display(Name = "Vehicle With Registration")]
        public string VehicleWithRegistration => $"{Brand} {Model} {Registration}";


        public ICollection<VehicleAssignment> VehicleAssignment { get; set; }
    }
}

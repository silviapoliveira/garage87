using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Name { get; set; }


        [Display(Name = "Country")]
        public int? CountryId { get; set; }


        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }
    }
}

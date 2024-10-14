using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Data.Entities
{
    public class Model : IEntity
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "Model")]
        public string ModelNumber { get; set; }


        [Display(Name = "Brand")]
        public int? BrandId { get; set; }


        [ForeignKey("BrandId")]
        public virtual Brand Brand { get; set; }
    }
}

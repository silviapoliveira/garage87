using System.ComponentModel.DataAnnotations;

namespace garage87.Data.Entities
{
    public class Brand : IEntity
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "Brand Name")]
        public string Name { get; set; }
    }
}

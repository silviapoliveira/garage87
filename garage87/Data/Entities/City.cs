using System.ComponentModel.DataAnnotations;

namespace garage87.Data.Entities
{
    public class City : IEntity
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Name { get; set; }
    }
}

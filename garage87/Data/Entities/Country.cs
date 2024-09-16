using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace garage87.Data.Entities
{
    public class Country : IEntity
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} characters.")]
        public string Name { get; set; }


        public ICollection<City> Cities { get; set; }


        [Display(Name = "Number of Cities")]
        public int NumberCities => Cities == null ? 0 : Cities.Count;
    }
}

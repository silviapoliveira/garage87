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


        [Display(Name = "Image")]
        public string ImageUrl { get; set; }


        public string Description { get; set; }


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

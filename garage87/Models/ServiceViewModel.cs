using garage87.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace garage87.Models
{
    public class ServiceViewModel
    {
        public int ServiceId { get; set; }


        [Required]
        [Display(Name = "Service")]
        [MaxLength(200, ErrorMessage = "The field {0} can contain {1} characters.")]
        public string Name { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }


        public string ImageUrl { get; set; }


        [Required]
        [Display(Name = "Service Detail")]
        public string Description { get; set; }


        public string ImageFullPath { get; set; }


        public IFormFile ImageFile { get; set; }


        public Service GetEntity(Service obj, string path)
        {
            if (obj == null) obj = new Service();
            obj.Name = this.Name;
            obj.Price = this.Price;
            obj.ImageUrl = path;
            obj.Description = this.Description;
            return obj;

        }

        public static ServiceViewModel FromEntity(Service service)
        {
            if (service == null) return null;

            return new ServiceViewModel
            {
                ServiceId = service.Id,
                Name = service.Name,
                Price = service.Price,
                Description = service.Description,
                ImageUrl = service.ImageUrl,
                ImageFullPath = service.ImageFullPath
            };
        }

    }
}

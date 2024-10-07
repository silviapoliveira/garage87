using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Data.Entities
{
    public class Customer : IEntity
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [MaxLength(50, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "")]
        public string ImageUrl { get; set; }


        [MaxLength(200, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        public string Address { get; set; }


        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }


        [Display(Name = "City")]
        public int CityId { get; set; }


        [ForeignKey("CityId")]
        public virtual City City { get; set; }


        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email should be something like example@example.com.")]
        public string Email { get; set; }


        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        [Required]
        [MaxLength(15, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "VAT Number")]
        public string VatNumber { get; set; }


        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";


        [Display(Name = "Full Address")]
        public string FullAddress => $"{Address}<br>{ZipCode} {City}";


        // List of vehicles associated to the customer
        public ICollection<Vehicle> Vehicles { get; set; }


        [Display(Name = "Number of Vehicles")]
        public int NumberVehicles => Vehicles == null ? 0 : Vehicles.Count;


        public string AddedBy { get; set; }


        public string UserId { get; set; }


        [ForeignKey("UserId")]
        public virtual User User { get; set; }


        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return "https://localhost:44331/images/customers/noimage.jpg";
                }

                return $"https://localhost:44331{ImageUrl.Substring(1)}";
            }
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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


        [Required]
        [MaxLength(200, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        public string Address { get; set; }


        [Required]
        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }


        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        public string City { get; set; }


        [Required]
        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        public string Country { get; set; }


        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email should be something like example@example.com.")]
        public string Email { get; set; }


        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        [Required]
        [MaxLength(15, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        [Display(Name = "VAT Number")]
        public string VatNumber { get; set; }


        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";


        [Display(Name = "Full Address")]
        public string FullAddress => $"{Address}<br>{ZipCode} {City}<br>{Country}";


        // List of vehicles associated to the customer
        public ICollection<Vehicle> Vehicles { get; set; }


        [Display(Name = "Number of Vehicles")]
        public int NumberVehicles => Vehicles == null ? 0 : Vehicles.Count;

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return "https://localhost:44361/images/customers/noimage.jpg";
                }

                return $"https://localhost:44361{ImageUrl.Substring(1)}";
            }
        }
    }
}

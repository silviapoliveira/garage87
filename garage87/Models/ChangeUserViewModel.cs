﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace garage87.Models
{
    public class ChangeUserViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Display(Name = "Username / Email")]
        public string Email { get; set; }


        [MaxLength(100, ErrorMessage = "The field {0} can only contain {1} characters length.")]
        public string Address { get; set; }


        [MaxLength(20, ErrorMessage = "The field {0} can only contain {1} characters length.")]
        public string PhoneNumber { get; set; }


        [Display(Name = "City")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a city.")]
        public int CityId { get; set; }


        public IEnumerable<SelectListItem> Cities { get; set; }


        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }


        [Display(Name = "VAT Number")]
        [MaxLength(15, ErrorMessage = "The field {0} can only contain {1} charaters.")]
        public string Vat { get; set; }
    }
}

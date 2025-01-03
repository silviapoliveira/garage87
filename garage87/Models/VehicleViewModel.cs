﻿using System.ComponentModel.DataAnnotations;

namespace garage87.Models
{
    public class VehicleViewModel
    {
        public int CustomerId { get; set; }


        public int VehicleId { get; set; }


        [Required]
        public string Type { get; set; }


        [Required]
        public int BrandId { get; set; }


        [Required]
        public int ModelId { get; set; }


        [Required]
        public string Registration { get; set; }


        [Required]
        public int Year { get; set; }


        [Required]
        public int Month { get; set; }
    }
}

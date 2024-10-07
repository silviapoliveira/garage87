using System;
using System.ComponentModel.DataAnnotations;

namespace garage87.Data.Entities
{
    public class Message : IEntity
    {
        public int Id { get; set; }


        [Required]
        public string Name { get; set; }


        [Required]
        public string Email { get; set; }


        [Display(Name = "Phone No")]
        public string Phone { get; set; }


        [Required]
        [Display(Name = "Message")]
        public string MessageDetail { get; set; }


        public DateTime MessageDate { get; set; }

    }
}

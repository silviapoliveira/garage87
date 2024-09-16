using System.ComponentModel.DataAnnotations;

namespace garage87.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

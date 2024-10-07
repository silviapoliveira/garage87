using garage87.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Models
{
    public class RepairDetailVM
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }


        public decimal ServiceCost { get; set; }


        public string ServiceName { get; set; }


        public int? RepairId { get; set; }


        [ForeignKey("RepairId")]
        public virtual Repair Repair { get; set; }


        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }


        public int Index { get; set; }


        public bool IsDeleted { get; set; }

    }
}

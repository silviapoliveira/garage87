using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Data.Entities
{
    public class RepairDetail : IEntity
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }


        public decimal ServiceCost { get; set; }


        public int? RepairId { get; set; }


        [ForeignKey("RepairId")]
        public virtual Repair Repair { get; set; }


        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
    }
}

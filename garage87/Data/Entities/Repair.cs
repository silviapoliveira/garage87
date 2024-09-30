using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Data.Entities
{
    public class Repair : IEntity
    {
        public int Id { get; set; }


        public int LabourHours { get; set; }


        public int? EmployeeId { get; set; }


        public int? VehicleId { get; set; }


        public int? VehicleServiceAssignmentId { get; set; }


        public decimal Total { get; set; }


        public DateTime RepairDate { get; set; }


        public string Detail { get; set; }


        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }


        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }


        [ForeignKey("VehicleServiceAssignmentId")]
        public virtual VehicleServiceAssignment VehicleServiceAssignment { get; set; }


        public ICollection<RepairDetail> RepairDetail { get; set; }
    }
}

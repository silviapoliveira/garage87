﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace garage87.Data.Entities
{
    public class VehicleAssignment : IEntity
    {
        public int Id { get; set; }


        public int? EmployeeId { get; set; }


        public int? ServiceId { get; set; }


        public int? VehicleId { get; set; }


        public DateTime TaskDate { get; set; }


        public int? Status { get; set; }


        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }


        [ForeignKey("VehicleId")]
        public virtual Vehicle Vehicle { get; set; }
    }
}

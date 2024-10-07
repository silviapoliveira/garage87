using garage87.Data.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace garage87.Models
{
    public class VehicleAssignmentVM
    {
        public int Id { get; set; }


        [Required]
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }


        [Required]
        [Display(Name = "Vehicle")]
        public int? VehicleId { get; set; }


        [Required]
        [Display(Name = "Repair Date")]
        public DateTime TaskDate { get; set; }


        [Required]
        [Display(Name = "Repair Staus")]
        public int? Status { get; set; }


        public VehicleAssignment GetEntity(VehicleAssignment obj)
        {
            if (obj == null) obj = new VehicleAssignment();
            obj.EmployeeId = this.EmployeeId;
            obj.VehicleId = this.VehicleId;
            obj.TaskDate = this.TaskDate;
            obj.Status = this.Status;
            return obj;

        }

        public static VehicleAssignmentVM FromEntity(VehicleAssignment obj)
        {
            if (obj == null) return null;

            return new VehicleAssignmentVM
            {
                Id = obj.Id,
                EmployeeId = obj.EmployeeId,
                VehicleId = obj.VehicleId,
                TaskDate = obj.TaskDate,
                Status = obj.Status,
            };
        }
    }
}

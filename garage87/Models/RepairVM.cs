using garage87.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace garage87.Models
{
    public class RepairVM
    {
        public RepairVM()
        {
            this.RepairDetail = new List<RepairDetailVM>();
        }
        public int Id { get; set; }


        [Required]
        [Display(Name = "Labour Hours")]
        public int LabourHours { get; set; }


        public int? EmployeeId { get; set; }


        public int? VehicleId { get; set; }


        [Required]
        public int? VehicleAssignmentId { get; set; }


        [Required]
        [Display(Name = "Total Amount")]
        public decimal Total { get; set; }


        [Display(Name = "Date")]
        [Required]
        public DateTime RepairDate { get; set; }


        [Required]
        [Display(Name = "Repair Description")]
        public string Detail { get; set; }


        [ValidateNever]
        public virtual Employee Employee { get; set; }


        [ValidateNever]
        public virtual Vehicle Vehicle { get; set; }


        public virtual List<RepairDetailVM> RepairDetail { get; set; }


        public Repair GetEntity(Repair obj)
        {

            if (obj == null) obj = new Repair();

            obj.LabourHours = this.LabourHours;
            obj.Total = this.Total;
            obj.RepairDate = this.RepairDate;
            obj.Detail = this.Detail;
            obj.VehicleId = this.VehicleId;
            obj.EmployeeId = this.EmployeeId;
            obj.VehicleAssignmentId = this.VehicleAssignmentId;

            if (obj.RepairDetail == null)
            {
                obj.RepairDetail = new List<RepairDetail>();
            }

            if (this.RepairDetail != null)
            {
                foreach (var rd in this.RepairDetail.Where(x => !x.IsDeleted))
                {

                    var newDetail = new RepairDetail
                    {
                        ServiceId = rd.ServiceId,
                        ServiceCost = rd.ServiceCost,

                    };

                    obj.RepairDetail.Add(newDetail);
                }
            }

            return obj;
        }

        public static RepairVM FromEntity(Repair obj)
        {
            if (obj == null) return null;

            var repairViewModel = new RepairVM
            {
                Id = obj.Id,
                LabourHours = obj.LabourHours,
                Total = obj.Total,
                RepairDate = obj.RepairDate,
                VehicleAssignmentId = obj.VehicleAssignmentId,
                Detail = obj.Detail,
                EmployeeId = obj.VehicleAssignment.EmployeeId,
                VehicleId = obj.VehicleAssignment.VehicleId,
                Vehicle = obj.Vehicle,
                Employee = obj.Employee,
                // Map the RepairDetail
                RepairDetail = obj.RepairDetail != null ?
                               obj.RepairDetail.Select(rd => new RepairDetailVM
                               {
                                   Id = rd.Id,
                                   RepairId = rd.RepairId,
                                   ServiceName = rd.Service?.Name,
                                   ServiceId = rd.ServiceId,
                                   ServiceCost = rd.ServiceCost,
                                   // Index will be updated later
                               }).ToList() :
                               new List<RepairDetailVM>()
            };

            // Update the indexes after mapping
            repairViewModel.UpdateModel();

            return repairViewModel;
        }

        // Update the index for each RepairDetail
        private void UpdateModel()
        {
            var ind = 0;
            foreach (var item in this.RepairDetail)
            {
                item.Index = ind;
                ind++;
            }
        }

    }
}

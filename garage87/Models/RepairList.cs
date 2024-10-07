using garage87.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace garage87.Models
{
    public class RepairList
    {
        public RepairList()
        {

            this.RepairVM = new List<RepairVM>();

        }


        public int? EmployeeId { get; set; }


        public int? VehicleId { get; set; }


        public int? ServiceId { get; set; }


        public DateTime? Date { get; set; }


        public List<RepairVM> RepairVM { get; set; }


        public static RepairList FromEntity(IEnumerable<Repair> repairs)
        {
            if (repairs == null || !repairs.Any()) return null;

            return new RepairList
            {
                RepairVM = repairs.Select(obj => new RepairVM
                {
                    Id = obj.Id,
                    LabourHours = obj.LabourHours,
                    Total = obj.Total,
                    RepairDate = obj.RepairDate,
                    VehicleId = obj.VehicleId,
                    Vehicle = obj.Vehicle,
                    Employee = obj.Employee,
                    EmployeeId = obj.EmployeeId,
                    VehicleAssignmentId = obj.VehicleAssignmentId,
                    RepairDetail = obj.RepairDetail?.Select(rd => new RepairDetailVM
                    {
                        Id = rd.Id,
                        ServiceCost = rd.ServiceCost,
                        ServiceId = rd.ServiceId,
                        ServiceName = rd.Service?.Name,
                    }).ToList() ?? new List<RepairDetailVM>()
                }).ToList()
            };
        }
    }
}

using garage87.Data.Entities;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace garage87.Models
{
    public class VehicleAssignmentListVM
    {
        public int? EmployeeId { get; set; }


        public int? Status { get; set; }


        public DateTime? Date { get; set; }


        public List<VehicleAssignment> VehicleAssignment { get; set; }


        public static VehicleAssignmentListVM FromEntity(IEnumerable<VehicleAssignment> data)
        {
            if (data == null || !data.Any()) return null;

            return new VehicleAssignmentListVM
            {
                VehicleAssignment = data.Select(obj => new VehicleAssignment
                {
                    Id = obj.Id,
                    TaskDate = obj.TaskDate,
                    Status = obj.Status,
                    VehicleId = obj.VehicleId,
                    Vehicle = obj.Vehicle,
                    Employee = obj.Employee,
                    EmployeeId = obj.EmployeeId,

                }).ToList()
            };
        }
    }
}

using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    public class RepairController : Controller
    {
        public readonly IRepairRepository _repairRepo;
        public readonly IRepairDetailRepository _repairDetailRepo;
        public readonly IServiceRepository _serviceRepo;
        public readonly IVehicleAssignmentRepository _assignmentRepo;
        public readonly IEmployeeRepository _employeeRepo;
        public readonly IVehicleRepository _vehicleRepo;
        public RepairController(IRepairRepository repairRepo, IVehicleAssignmentRepository assignmentRepo, IServiceRepository serviceRepo, IEmployeeRepository employeeRepo, IVehicleRepository vehicleRepo, IRepairDetailRepository repairDetailRepo)
        {
            _repairRepo = repairRepo;
            _serviceRepo = serviceRepo;
            _assignmentRepo = assignmentRepo;
            _employeeRepo = employeeRepo;
            _vehicleRepo = vehicleRepo;
            _repairDetailRepo = repairDetailRepo;

        }

        public async Task<IActionResult> Index(RepairList obj)
        {
            ViewBag.Employees = new SelectList(_employeeRepo.GetAll().Where(x => x.Function == (int)Enums.EmployeeFunctionEnum.Mechanic), "Id", "FullName");
            ViewBag.Vehicles = new SelectList(_vehicleRepo.GetAll().OrderBy(s => s.Registration), "Id", "Registration");

            var data = _repairRepo.GetAll();
            var repair = data.Include(x => x.VehicleAssignment).Include(x => x.Vehicle).Include(x => x.Employee).Include(x => x.RepairDetail).ThenInclude(x => x.Service).ToList();
            if (obj.EmployeeId.HasValue)
                repair = repair.Where(x => x.EmployeeId == obj.EmployeeId).ToList();
            if (obj.VehicleId.HasValue)
                repair = repair.Where(x => x.VehicleId == obj.VehicleId).ToList();
            if (obj.Date.HasValue)
                repair = repair.Where(x => x.RepairDate == obj.Date).ToList();
            obj = RepairList.FromEntity(repair);

            return View(obj);
        }

        public async Task<IActionResult> AddRepairInvoice(int? assignid, int? repairId)
        {
            RepairVM model = new RepairVM();
            if (!assignid.HasValue && !repairId.HasValue)
            {
                return NotFound();
            }
            if (assignid.HasValue && assignid.Value != 0)
            {
                var assignment = await _assignmentRepo.GetByIdAsync((int)assignid);
                if (assignment == null)
                {
                    return NotFound();
                }

                var repair = _repairRepo.GetAll().Where(x => x.VehicleAssignmentId == assignid).Include(r => r.RepairDetail).Include(v => v.VehicleAssignment).FirstOrDefault();
                if (repair != null)
                {
                    model = RepairVM.FromEntity(repair);

                    // Populate ServiceName for each RepairDetail in the model
                    foreach (var detail in model.RepairDetail)
                    {
                        var service = await _serviceRepo.GetByIdAsync(detail.ServiceId);
                        if (service != null)
                        {
                            detail.ServiceName = service.Name;
                        }
                        else
                        {
                            detail.ServiceName = "";
                        }
                    }
                }
                model.VehicleAssignmentId = assignment.Id;
                model.EmployeeId = assignment.EmployeeId;
                model.VehicleId = assignment.VehicleId;
            }
            if (repairId.HasValue && repairId.Value != 0)
            {
                var repair = await _repairRepo.GetByIdAsync(repairId.Value, r => r.RepairDetail, v => v.VehicleAssignment);
                //var repair =  _repairRepo.GetAll().Where(x=>x.VehicleAssignmentId==assignid).Include(r=>r.RepairDetail).Include(v=>v.VehicleAssignment).FirstOrDefault();

                if (repair == null)
                {
                    return NotFound();
                }
                model = RepairVM.FromEntity(repair);

                // Populate ServiceName for each RepairDetail in the model
                foreach (var detail in model.RepairDetail)
                {
                    var service = await _serviceRepo.GetByIdAsync(detail.ServiceId);
                    if (service != null)
                    {
                        detail.ServiceName = service.Name;
                    }
                    else
                    {
                        detail.ServiceName = "";
                    }
                }

            }

            ViewBag.Services = new SelectList(_serviceRepo.GetAll().OrderBy(s => s.Name), "Id", "Name");

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddRepairInvoice(int? Id, RepairVM data)
        {
            if (data.RepairDetail == null || !data.RepairDetail.Any() ||
        data.RepairDetail.All(rd => rd.IsDeleted) ||
        (data.RepairDetail.Count == 1 && data.RepairDetail.First().IsDeleted))
            {
                ModelState.AddModelError(string.Empty, "At least one repair detail is required and must not be marked as deleted.");
            }

            decimal repairDetailTotal = data.RepairDetail
       .Where(rd => !rd.IsDeleted) // Only include non-deleted items
       .Sum(rd => rd.ServiceCost);

            if (data.Total == 0 || data.Total < repairDetailTotal)
            {
                ModelState.AddModelError("Total", "The total amount must be greater than or equal to the sum of the service costs.");
            }
            if (data.LabourHours == 0)
            {
                ModelState.AddModelError("LabourHours", "Labour Hours can't be 0.");
            }

            ViewBag.Services = new SelectList(_serviceRepo.GetAll(), "Id", "Name");
            if (!ModelState.IsValid)
            {
                return View(data);
            }

            var repairEntity = Id.HasValue && Id.Value != 0 ? await _repairRepo.GetByIdAsync(Id.Value, r => r.RepairDetail, v => v.VehicleAssignment) : new Repair();
            if (repairEntity != null && repairEntity.RepairDetail != null)
            {
                var detailsToRemove = repairEntity.RepairDetail
          .Where(existingDetail => data.RepairDetail.Any(rd => rd.Id == existingDetail.Id && rd.IsDeleted))
          .ToList();

                foreach (var detail in detailsToRemove)
                {
                    await _repairDetailRepo.DeleteAsync(detail);
                }
            }

            repairEntity = data.GetEntity(repairEntity);

            // Save the changes or new entity based on the condition
            if (Id.HasValue && Id.Value != 0)
            {
                var assignment = await _assignmentRepo.GetByIdAsync((int)data.VehicleAssignmentId);
                if (assignment != null)
                {

                    assignment.Status = (int)Enums.RepairStatusEnum.Invoiced;
                    await _assignmentRepo.UpdateAsync(assignment);
                }
                await _repairRepo.UpdateAsync(repairEntity);
            }
            else
            {
                var assignment = await _assignmentRepo.GetByIdAsync((int)data.VehicleAssignmentId);
                if (assignment != null)
                {
                    assignment.Status = (int)Enums.RepairStatusEnum.Invoiced;
                    await _assignmentRepo.UpdateAsync(assignment);
                }
                await _repairRepo.CreateAsync(repairEntity);
            }

            return RedirectToAction("Index", "Repair");
        }

        [HttpGet]
        public IActionResult GetRepairDetails(int repairId)
        {
            var repair = _repairRepo.GetAll()
                .Include(r => r.Vehicle)
                .Include(r => r.RepairDetail)
                .ThenInclude(rd => rd.Service)
                .FirstOrDefault(r => r.Id == repairId);

            if (repair == null)
            {
                return NotFound();
            }
            var result = new
            {
                VehicleRegistration = repair.Vehicle.Registration,
                Total = repair.Total,
                details = repair.RepairDetail.Select(rd => new
                {
                    rd.Service.Name,
                    rd.ServiceCost
                }).ToList()
            };

            return Json(result);
        }

        public async Task<IActionResult> AddDetail(int ind, int serviceId, string servicename, int? RepairId, int? Id = null)
        {
            var data = await _serviceRepo.GetByIdAsync(serviceId);
            int finalDetailId = Id ?? 0;
            int finalRepairId = RepairId ?? 0;
            decimal finalCost = data.Price;
            var model = new RepairDetailVM { Index = ind, ServiceId = serviceId, Id = finalDetailId, ServiceCost = finalCost, RepairId = RepairId, ServiceName = servicename };
            return PartialView("_RepairDetail", model);

        }
    }
}

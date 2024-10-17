using AspNetCoreHero.ToastNotification.Abstractions;
using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    public class VehicleAssignmentController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IVehicleAssignmentRepository _vehicleAssignmentRepository;
        private readonly INotyfService _notyf;
        public VehicleAssignmentController(IVehicleRepository vehicleRepository, IServiceRepository serviceRepository, IEmployeeRepository employeeRepository, IVehicleAssignmentRepository vehicleAssignmentRepository, INotyfService notyf, IUserHelper userHelper, IMailHelper mailHelper)
        {
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
            _employeeRepository = employeeRepository;
            _vehicleAssignmentRepository = vehicleAssignmentRepository;
            _notyf = notyf;
            _userHelper = userHelper;
            _mailHelper = mailHelper;

        }

        public IActionResult Index(VehicleAssignmentListVM obj)
        {

            var services = _vehicleAssignmentRepository.GetAll();
            services = services.Include(e => e.Employee)
                               .Include(s => s.Vehicle)
                               .ThenInclude(c => c.Customer);
            if (obj.Status.HasValue)
                services = services.Where(x => x.Status == obj.Status.Value);

            if (obj.EmployeeId.HasValue)
                services = services.Where(x => x.EmployeeId == obj.EmployeeId);
            if (obj.Date.HasValue)
                services = services.Where(x => x.TaskDate.Date == obj.Date.Value.Date);

            obj = VehicleAssignmentListVM.FromEntity(services);
            ViewBag.Employees = new SelectList(_employeeRepository.GetAll().Where(x => x.Function == (int)Enums.EmployeeFunctionEnum.Mechanic), "Id", "FullName");

            return View(obj);
        }

        public IActionResult CompletedRepairs(VehicleAssignmentListVM obj)
        {
            // Start with the base query without chaining Includes
            var services = _vehicleAssignmentRepository.GetAll();

            // Apply Includes separately
            services = services.Include(e => e.Employee)
                               .Include(s => s.Vehicle)
                               .ThenInclude(c => c.Customer).Where(x => x.Status == (int)Enums.RepairStatusEnum.Completed);
            if (obj.EmployeeId.HasValue)
                services = services.Where(x => x.EmployeeId == obj.EmployeeId);
            if (obj.Date.HasValue)
                services = services.Where(x => x.TaskDate.Date == obj.Date.Value.Date);
            obj = VehicleAssignmentListVM.FromEntity(services);
            ViewBag.Employees = new SelectList(_employeeRepository.GetAll().Where(x => x.Function == (int)Enums.EmployeeFunctionEnum.Mechanic), "Id", "FullName");

            return View(obj);
        }

        public IActionResult AddVehicleAssignment()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicleAssignment(VehicleAssignmentVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.TaskDate < DateTime.Now.Date)
                {
                    ModelState.AddModelError("TaskDate", "Task date cannot be in the past.");
                    _notyf.Warning("Repair not Assigned!");
                    return View(model);
                }

                if (model.TaskDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    ModelState.AddModelError("TaskDate", "Task date cannot be on a Sunday.");
                    _notyf.Warning("Repair not Assigned!");
                    return View(model);
                }
                var obj = model.GetEntity(null);
                await _vehicleAssignmentRepository.CreateAsync(obj);
                _notyf.Success("Repair Assigned successfully!");
                return RedirectToAction("Index", "VehicleAssignment");
            }
            _notyf.Warning("Repair not Assigned!");
            return View(model);
        }

        public async Task<IActionResult> EditVehicleAssignment(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = await _vehicleAssignmentRepository.GetByIdAsync((int)id);

            if (data == null)
            {
                return NotFound();
            }
            var ViewModel = VehicleAssignmentVM.FromEntity(data);
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicleAssignment(int id, VehicleAssignmentVM model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (model.TaskDate < DateTime.Now.Date)
                {
                    ModelState.AddModelError("TaskDate", "Task date cannot be in the past.");
                    _notyf.Warning("Assigned Service not updated!");
                    return View(model);
                }

                if (model.TaskDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    ModelState.AddModelError("TaskDate", "Task date cannot be on a Sunday.");
                    _notyf.Warning("Assigned Service not updated!");
                    return View(model);
                }
                var data = await _vehicleAssignmentRepository.GetByIdAsync((int)id);
                var obj = model.GetEntity(data);
                await _vehicleAssignmentRepository.UpdateAsync(obj);
                _notyf.Success("Assigned Service updated successfully!");
                return RedirectToAction("Index", "VehicleAssignment");
            }
            _notyf.Warning("Assigned Service not updated!");
            return View(model);
        }
        public async Task<IActionResult> UpdateStatus(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var data = await _vehicleAssignmentRepository.GetByIdAsync((int)id);

            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, VehicleAssignment model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }


            var data = await _vehicleAssignmentRepository.GetByIdAsync((int)id);
            data.Status = model.Status;
            await _vehicleAssignmentRepository.UpdateAsync(data);
            if (model.Status == (int)Enums.RepairStatusEnum.Cancelled)
            {
                var Customer = _vehicleRepository.GetAll().Include(x => x.Customer).Where(x => x.Id == data.VehicleId).FirstOrDefault();
                var user = await _userHelper.GetUserByIdAsync(Customer.Customer.UserId);
                var userEmail = user?.Email;
                string messageBody = $"<h1>Appointment Cancellation Notice</h1>" +
                       $"<p>Dear user,</p>" +
                       $"<p>We regret to inform you that your appointment scheduled with us has been canceled.</p>" +
                       $"<p>If you have any questions, please feel free to contact us.</p>" +
                       $"<p>Thank you for your understanding!</p>";
                try
                {
                    // Send the email
                    Response response = _mailHelper.SendEmail(userEmail, "Appointment Reminder", messageBody);
                    if (!response.IsSuccess)
                    {
                        _notyf.Error($"Email sending failed to customer: {userEmail}");
                    }
                }
                catch (Exception ex)
                {
                    _notyf.Error($"Email sending failed to customer: {userEmail}");
                }
            }
            _notyf.Success("Assigned Service updated successfully!");
            return RedirectToAction("Index", "VehicleAssignment");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var getData = await _vehicleAssignmentRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _vehicleAssignmentRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Record deleted successfully" });
                    else
                        return Json(new { success = false, message = "The data cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Record not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the record. Please try again." });
            }
        }

        public IActionResult GetList([FromBody] DataManagerRequest dm)
        {
            IQueryable<VehicleAssignment> Data = _vehicleAssignmentRepository.GetAll().Include(e => e.Employee).Include(s => s.Vehicle).ThenInclude(c => c.Customer);
            DataOperations operation = new DataOperations();
            var count = Data.Count();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                Data = operation.PerformSearching(Data, dm.Search);
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                Data = operation.PerformSorting(Data, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0)
            {
                Data = operation.PerformFiltering(Data, dm.Where, dm.Where[0].Operator);
            }
            if (dm.Skip != 0)
            {
                Data = operation.PerformSkip(Data, dm.Skip);
            }
            if (dm.Take != 0)
            {
                Data = operation.PerformTake(Data, dm.Take);
            }
            var list = Data.ToList();
            return dm.RequiresCounts ? Json(new { items = list, result = list, count = count }) : Json(list);
        }

        public ActionResult VehicleDropdown([FromBody] DataManagerRequest dm)
        {
            var Data = _vehicleRepository.GetAll()
                .Include(c => c.Customer)
                .Include(b => b.Brand)
                .Include(m => m.Model)
                .AsQueryable();

            Data = Data.OrderBy(v => v.Customer.FirstName);

            var count = Data.Count();
            DataOperations operation = new DataOperations();

            if (dm.Where != null && dm.Where.Count > 0)
            {
                // Perform filtering only if there are conditions
                Data = operation.PerformFiltering(Data, dm.Where, dm.Where[0].Operator);
            }

            if (dm.Skip != 0)
            {
                Data = operation.PerformSkip(Data, dm.Skip);
            }
            if (dm.Take != 0)
            {
                Data = operation.PerformTake(Data, dm.Take);
            }

            var list = Data.ToList();
            return dm.RequiresCounts ? Json(new { items = list, result = list, count = count }) : Json(list);
        }

        public ActionResult ServicesDropdown([FromBody] DataManagerRequest dm)
        {
            var Data = _serviceRepository.GetAll();
            var count = Data.Count();
            Data = Data.OrderBy(v => v.Name);
            DataOperations operation = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                // Perform filtering only if there are conditions
                Data = operation.PerformFiltering(Data, dm.Where, dm.Where[0].Operator);
            }

            if (dm.Skip != 0)
            {
                Data = operation.PerformSkip(Data, dm.Skip);
            }
            if (dm.Take != 0)
            {
                Data = operation.PerformTake(Data, dm.Take);
            }
            var list = Data.ToList();
            return dm.RequiresCounts ? Json(new { items = list, result = list, count = count }) : Json(list);
        }

        public ActionResult EmployeeDropdown([FromBody] DataManagerRequest dm)
        {
            var Data = _employeeRepository.GetAll().Include(s => s.Specialities).Include(x => x.User).Where(x => x.Function == (int)Enums.EmployeeFunctionEnum.Mechanic);
            var count = Data.Count();
            DataOperations operation = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0)
            {
                // Perform filtering only if there are conditions
                Data = operation.PerformFiltering(Data, dm.Where, dm.Where[0].Operator);
            }

            if (dm.Skip != 0)
            {
                Data = operation.PerformSkip(Data, dm.Skip);
            }
            if (dm.Take != 0)
            {
                Data = operation.PerformTake(Data, dm.Take);
            }
            var list = Data.ToList();
            return dm.RequiresCounts ? Json(new { items = list, result = list, count = count }) : Json(list);
        }
    }
}

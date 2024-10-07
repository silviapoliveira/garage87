using AspNetCoreHero.ToastNotification.Abstractions;
using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    public class MechanicController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IVehicleAssignmentRepository _vehicleAssignmentRepository;
        private readonly INotyfService _notyf;
        public MechanicController(IVehicleRepository vehicleRepository, IServiceRepository serviceRepository, IEmployeeRepository employeeRepository, IVehicleAssignmentRepository vehicleAssignmentRepository, INotyfService notyf, IUserHelper userHelper)
        {
            _vehicleRepository = vehicleRepository;
            _serviceRepository = serviceRepository;
            _employeeRepository = employeeRepository;
            _vehicleAssignmentRepository = vehicleAssignmentRepository;
            _notyf = notyf;
            _userHelper = userHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MyTasks(VehicleAssignmentListVM obj)
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var services = _vehicleAssignmentRepository.GetAll().Where(x => x.Employee.UserId == user.Id && x.Status != (int)Enums.RepairStatusEnum.Cancelled);

            // Apply Includes separately
            services = services.Include(e => e.Employee)
                               .Include(s => s.Vehicle)
                               .ThenInclude(c => c.Customer);

            // Apply filter if Status has a value
            if (obj.Status.HasValue)
                services = services.Where(x => x.Status == obj.Status.Value);

            // You can apply additional filters like Date if needed
            if (obj.Date.HasValue)
                services = services.Where(x => x.TaskDate.Date == obj.Date.Value.Date);
            obj = VehicleAssignmentListVM.FromEntity(services);
            return View(obj);
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
            _notyf.Success("Assigned Service updated successfully!");
            return RedirectToAction("MyTasks", "Mechanic");
        }
    }
}

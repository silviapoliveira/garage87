using AspNetCoreHero.ToastNotification.Abstractions;
using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeRepository _employeeService;
        private readonly IMessageRepository _messageService;
        private readonly ICustomerRepository _customerService;
        private readonly IServiceRepository _serviceService;
        private readonly IRepairRepository _repairService;
        private readonly IVehicleRepository _vehicleService;
        private readonly INotyfService _notyf;
        private readonly IUserHelper _userHelper;

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository employeeService, IServiceRepository serviceService, ICustomerRepository customerService, IMessageRepository messageService, INotyfService notyf, IRepairRepository repairService, IVehicleRepository vehicleService, IUserHelper userHelper)
        {
            _logger = logger;
            _employeeService = employeeService;
            _serviceService = serviceService;
            _customerService = customerService;
            _messageService = messageService;
            _notyf = notyf;
            _repairService = repairService;
            _vehicleService = vehicleService;
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            var mechanics = _employeeService.GetAll().Where(x => x.Function == (int)Enums.EmployeeFunctionEnum.Mechanic);
            var customers = _customerService.GetAll();
            ViewBag.mechanics = mechanics.Count();
            ViewBag.Customers = customers.Count();
            var services = _serviceService.GetAll().Take(6);
            var homeVM = new HomeVM
            {
                Mechanics = mechanics,
                Services = services
            };
            return View(homeVM);
        }

        public IActionResult Services()
        {
            var services = _serviceService.GetAll();

            return View(services);
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(Message obj)
        {
            if (ModelState.IsValid)
            {
                obj.MessageDate = DateTime.Now;
                _messageService.CreateAsync(obj);
                _notyf.Success("Message Sent Successfully!");
                return RedirectToAction("Contact", "Home");
            }
            _notyf.Error("Message Sending failed!");
            return View(obj);
        }
        [Authorize]
        public async Task<IActionResult> RepairHistory(RepairList obj)
        {


            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            var customer = _customerService.GetAll().Where(x => x.UserId == user.Id).FirstOrDefault();
            var data = _repairService.GetAll();
            var repair = data.Include(x => x.VehicleAssignment).Include(x => x.Vehicle).Include(x => x.Employee).Include(x => x.RepairDetail).ThenInclude(x => x.Service).ToList();
            repair = repair.Where(x => x.Vehicle.CustomerId == customer.Id).ToList();
            if (obj.VehicleId.HasValue)
                repair = repair.Where(x => x.VehicleId == obj.VehicleId).ToList();
            if (obj.Date.HasValue)
                repair = repair.Where(x => x.RepairDate == obj.Date).ToList();
            obj = RepairList.FromEntity(repair);
            ViewBag.Vehicles = new SelectList(_vehicleService.GetAll().Where(x => x.CustomerId == customer.Id), "Id", "Registration");
            return View(obj);
        }

        [Authorize]
        [HttpGet]

        public IActionResult GetRepairDetails(int repairId)
        {
            var repair = _repairService.GetAll()
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
                details = repair.RepairDetail.Select(rd => new
                {
                    rd.Service.Name,
                    rd.ServiceCost
                }).ToList()
            };

            return Json(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

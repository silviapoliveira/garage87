using garage87.Data.Repositories;
using garage87.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IServiceRepository _serviceService;

        public HomeController(ILogger<HomeController> logger, IEmployeeRepository employeeService, IServiceRepository serviceService)
        {
            _logger = logger;
            _employeeService = employeeService;
            _serviceService = serviceService;
        }

        public IActionResult Index()
        {
            var mechanics = _employeeService.GetAll().Where(x => x.Function == (int)Enums.EmployeeFunctionEnum.Mechanic);
            var services = _serviceService.GetAll();
            var homeVM = new HomeVM
            {
                Mechanics = mechanics,
                Services = services
            };
            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

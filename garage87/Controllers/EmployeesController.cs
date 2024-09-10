using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using garage87.Data;
using garage87.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Vereyon.Web;
using garage87.Data.Repositories;
using garage87.Models;

namespace garage87.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFlashMessage _flashMessage;

        public EmployeesController
            (IEmployeeRepository employeeRepository,
            IFlashMessage flashMessage)
        {
            _employeeRepository = employeeRepository;
            _flashMessage = flashMessage;
        }

        public IActionResult Index()
        {
            return View(_employeeRepository.GetEmployeesWithServices());
        }

        public async Task<IActionResult> DeleteService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _employeeRepository.GetServiceAsync(id.Value);
            if (service == null)
            {
                return NotFound();
            }

            var employeeId = await _employeeRepository.DeleteServiceAsync(service);
            return this.RedirectToAction($"Details", new { id = employeeId });
        }

        public async Task<IActionResult> EditService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _employeeRepository.GetServiceAsync(id.Value);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(Service service)
        {
            if (this.ModelState.IsValid)
            {
                var employeeId = await _employeeRepository.UpdateServiceAsync(service);
                if (employeeId != 0)
                {
                    return this.RedirectToAction($"Details", new { id = employeeId });
                }
            }

            return this.View(service);
        }

        public async Task<IActionResult> AddService(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            var model = new ServiceViewModel { EmployeeId = employee.Id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddService(ServiceViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await _employeeRepository.AddServiceAsync(model);
                return RedirectToAction("Details", new { id = model.EmployeeId });
            }

            return this.View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeWithServicesAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeRepository.CreateAsync(employee);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("This employee already exists.");
                }

                return View(employee);
            }

            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _employeeRepository.UpdateAsync(employee);
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            await _employeeRepository.DeleteAsync(employee);
            return RedirectToAction(nameof(Index));
        }
    }
}

using garage87.Data.Entities;
using garage87.Data.Repositories;
using garage87.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace garage87.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IFlashMessage _flashMessage;

        public CustomersController
            (ICustomerRepository customerRepository,
            IFlashMessage flashMessage)
        {
            _customerRepository = customerRepository;
            _flashMessage = flashMessage;
        }

        // GET: Customers
        public IActionResult Index()
        {
            return View(_customerRepository.GetCustomersWithVehicles());
        }

        public async Task<IActionResult> DeleteVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _customerRepository.GetVehicleAsync(id.Value);
            if (vehicle == null)
            {
                return NotFound();
            }

            var customerId = await _customerRepository.DeleteVehicleAsync(vehicle);
            return this.RedirectToAction($"Details", new { id = customerId });
        }

        public async Task<IActionResult> EditVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _customerRepository.GetVehicleAsync(id.Value);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> EditVehicle(Vehicle vehicle)
        {
            if (this.ModelState.IsValid)
            {
                var customerId = await _customerRepository.UpdateVehicleAsync(vehicle);
                if (customerId != 0)
                {
                    return this.RedirectToAction($"Details", new { id = customerId });
                }
            }

            return this.View(vehicle);
        }

        public async Task<IActionResult> AddVehicle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }
            var model = new VehicleViewModel { CustomerId = customer.Id };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddVehicle(VehicleViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                await _customerRepository.AddVehicleAsync(model);
                return RedirectToAction("Details", new { id = model.CustomerId });
            }

            return this.View(model);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetCustomerWithVehiclesAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _customerRepository.CreateAsync(customer);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("This customer already exists.");
                }

                return View(customer);
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await _customerRepository.UpdateAsync(customer);
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            await _customerRepository.DeleteAsync(customer);
            return RedirectToAction(nameof(Index));
        }
    }
}

using garage87.Data.Entities;
using garage87.Data.Repositories;
using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Vereyon.Web;

namespace garage87.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;

        public CustomersController
            (ICustomerRepository customerRepository,
            IImageHelper imageHelper,
            IConverterHelper converterHelper,
            IFlashMessage flashMessage)
        {
            _customerRepository = customerRepository;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
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
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = string.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "customers");
                    }

                    var customer = _converterHelper.ToCustomer(model, path, true);

                    await _customerRepository.CreateAsync(customer);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("This customer already exists. Please remember VAT number must be unique.");
                }

                return View(model);
            }

            return View(model);
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

            var model = _converterHelper.ToCustomerViewModel(customer);

            return View(model);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "customers");
                    }

                    var customer = _converterHelper.ToCustomer(model, path, false);

                    await _customerRepository.UpdateAsync(customer);
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("Error! Please check the information. Remember VAT number must be unique.");
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
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

using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly IModelRepository _modelRepository;
        public VehiclesController(IVehicleRepository vehicleRepository, IBrandRepository brandRepository, IModelRepository modelRepository)
        {
            _vehicleRepository = vehicleRepository;
            _brandRepository = brandRepository;
            _modelRepository = modelRepository;

        }

        [Authorize(Roles = "Employee,Mechanic")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Employee,Mechanic")]
        public IActionResult Create()
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            ViewBag.Models = new SelectList(_modelRepository.GetAll(), "Id", "ModelNumber");
            return View();
        }

        [Authorize(Roles = "Employee,Mechanic")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            ViewBag.Models = new SelectList(_modelRepository.GetAll(), "Id", "ModelNumber");
            if (vehicle.CustomerId <= 0)
            {
                ModelState.AddModelError("CustomerId", "Please select customer.");
                return View(vehicle);
            }

            if (ModelState.IsValid)
            {
                var vehicles = _vehicleRepository.GetAll();
                bool exists = vehicles.Any(c => c.Registration.ToLower() == vehicle.Registration.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Registration", "A vehicle with the same registration already exists.");
                    return View(vehicle);
                }
                await _vehicleRepository.CreateAsync(vehicle);
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        [Authorize(Roles = "Employee,Mechanic")]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            ViewBag.Models = new SelectList(_modelRepository.GetAll(), "Id", "ModelNumber");
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _vehicleRepository.GetByIdAsync((int)id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        [Authorize(Roles = "Employee,Mechanic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            ViewBag.Models = new SelectList(_modelRepository.GetAll(), "Id", "ModelNumber");
            if (vehicle.CustomerId <= 0)
            {
                ModelState.AddModelError("CustomerId", "Please select customer.");
                return View(vehicle);
            }

            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var vehicles = _vehicleRepository.GetAll().Where(x => x.Id != vehicle.Id);
                    bool exists = vehicles.Any(c => c.Registration.ToLower() == vehicle.Registration.ToLower());

                    if (exists)
                    {
                        ModelState.AddModelError("Registration", "A vehicle with the same registration already exists.");
                        return View(vehicle);
                    }
                    await _vehicleRepository.UpdateAsync(vehicle);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        [Authorize(Roles = "Employee,Mechanic")]
        [HttpPost]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            try
            {
                var getData = await _vehicleRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _vehicleRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Vehicle deleted successfully" });
                    else
                        return Json(new { success = false, message = "The vehicle cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! vehicle not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the vehicle. Please try again." });
            }
        }

        private bool VehicleExists(int id)
        {
            return _vehicleRepository.GetAll().Any(e => e.Id == id);
        }

        #region Vechicle API methods
        public ActionResult VehiclesDropdown([FromBody] DataManagerRequest dm)
        {
            var Data = _vehicleRepository.GetAll();
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

        public IActionResult GetList([FromBody] DataManagerRequest dm)
        {
            IQueryable<Vehicle> Data = _vehicleRepository.GetAll().Include(x => x.Customer).Include(b => b.Brand).Include(m => m.Model);
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

        #endregion

        public IActionResult GetModelsByBrand(int brandId)
        {
            // Fetch models based on brandId
            var models = _modelRepository.GetAll().Where(x => x.BrandId == brandId);

            // Return as JSON
            return Json(models.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.ModelNumber
            }));
        }
    }
}

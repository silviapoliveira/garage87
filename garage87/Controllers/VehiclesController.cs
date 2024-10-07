using garage87.Data;
using garage87.Data.Entities;
using garage87.Data.Repositories;
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
        public VehiclesController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        [Authorize(Roles = "Employee")]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Employee")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                await _vehicleRepository.CreateAsync(vehicle);
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> Edit(int? id)
        {
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
        [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        [Authorize(Roles = "Employee")]
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
                        return Json(new { success = false, message = "The Vehicle cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Vehicle not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the Vehicle. Please try again." });
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
            IQueryable<Vehicle> Data = _vehicleRepository.GetAll().Include(x => x.Customer);
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
    }
}

using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BrandController : Controller
    {

        private readonly IBrandRepository _brandRepository;
        public BrandController(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<IActionResult> Index()
        {
            var obj = _brandRepository.GetAll();
            return View(obj);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Brand obj)
        {
            if (ModelState.IsValid)
            {
                var brands = _brandRepository.GetAll();
                bool exists = brands.Any(c => c.Name.ToLower() == obj.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "A brand with same name already exists.");
                    return View(obj);
                }
                await _brandRepository.CreateAsync(obj);
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            var obj = await _brandRepository.GetByIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Brand obj)
        {
            if (id != obj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var brands = _brandRepository.GetAll().Where(x => x.Id != obj.Id);
                    bool exists = brands.Any(c => c.Name.ToLower() == obj.Name.ToLower());

                    if (exists)
                    {
                        ModelState.AddModelError("Name", "A brand with same name already exists.");
                        return View(obj);
                    }
                    var data = await _brandRepository.GetByIdAsync(id);
                    if (data != null)
                    {
                        await _brandRepository.UpdateAsync(obj);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Record not found.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(obj.Id))
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
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            try
            {
                var getData = await _brandRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _brandRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Brand deleted successfully" });
                    else
                        return Json(new { success = false, message = "The brand cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Brand not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the brand. Please try again." });
            }
        }

        private bool BrandExists(int id)
        {
            return _brandRepository.GetAll().Any(e => e.Id == id);
        }
    }
}

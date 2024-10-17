using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ModelController : Controller
    {

        private readonly IModelRepository _modelRepository;
        private readonly IBrandRepository _brandRepository;
        public ModelController(IModelRepository modelRepository, IBrandRepository brandRepository)
        {
            _modelRepository = modelRepository;
            _brandRepository = brandRepository;

        }

        public async Task<IActionResult> Index()
        {
            var obj = _modelRepository.GetAll().Include(x => x.Brand);
            return View(obj);
        }

        public IActionResult Create()
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Model obj)
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            if (ModelState.IsValid)
            {
                if (obj.BrandId <= 0)
                {
                    ModelState.AddModelError("BrandId", "Please select brand.");
                    return View(obj);
                }
                var models = _modelRepository.GetAll();
                bool exists = models.Any(c => c.ModelNumber.ToLower() == obj.ModelNumber.ToLower() &&
                                c.BrandId == obj.BrandId);

                if (exists)
                {
                    ModelState.AddModelError("ModelNumber", "A model with the same brand already exists.");
                    return View(obj);
                }
                await _modelRepository.CreateAsync(obj);
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        public async Task<IActionResult> Edit(int id)
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            var obj = await _modelRepository.GetByIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Model obj)
        {
            ViewBag.Brands = new SelectList(_brandRepository.GetAll(), "Id", "Name");
            if (id != obj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var models = _modelRepository.GetAll().Where(x => x.Id != obj.Id);
                    bool exists = models.Any(c => c.ModelNumber.ToLower() == obj.ModelNumber.ToLower() &&
                                    c.BrandId == obj.BrandId);

                    if (exists)
                    {
                        ModelState.AddModelError("ModelNumber", "A model with the same brand already exists.");
                        return View(obj);
                    }
                    var data = await _modelRepository.GetByIdAsync(id);
                    if (data != null)
                    {
                        await _modelRepository.UpdateAsync(obj);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Record not found.");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModelExists(obj.Id))
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
        public async Task<IActionResult> DeleteModel(int id)
        {
            try
            {
                var getData = await _modelRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _modelRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Model deleted successfully" });
                    else
                        return Json(new { success = false, message = "The model cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Model not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the model. Please try again." });
            }
        }

        private bool ModelExists(int id)
        {
            return _modelRepository.GetAll().Any(e => e.Id == id);
        }
    }
}

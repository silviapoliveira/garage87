using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using garage87.Data;
using garage87.Data.Entities;
using garage87.Data.Repositories;
using garage87.Models;
using garage87.Helpers;
using garage87.Data.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace garage87.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecialitiesController : Controller
    {

        private readonly ISpecialitiesRepository _SpecialitiesRepository;

        public SpecialitiesController(ISpecialitiesRepository SpecialitiesRepository)
        {
            _SpecialitiesRepository = SpecialitiesRepository;
        }

        public async Task<IActionResult> Index()
        {
            var obj = _SpecialitiesRepository.GetAll();
            return View(obj);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specialities obj)
        {
            if (ModelState.IsValid)
            {
                var sep = _SpecialitiesRepository.GetAll();
                bool exists = sep.Any(c => c.Name.ToLower() == obj.Name.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "A Speciality with the same name already exists.");
                    return View(obj);
                }
                await _SpecialitiesRepository.CreateAsync(obj);
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

            var data = await _SpecialitiesRepository.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Specialities data)
        {
            if (id != data.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var sep = _SpecialitiesRepository.GetAll().Where(x => x.Id != data.Id);
                    bool exists = sep.Any(c => c.Name.ToLower() == data.Name.ToLower());

                    if (exists)
                    {
                        ModelState.AddModelError("Name", "A Speciality with the same name already exists.");
                        return View(data);
                    }
                    var obj = await _SpecialitiesRepository.GetByIdAsync(id);

                    await _SpecialitiesRepository.UpdateAsync(data);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialityExists(data.Id))
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
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSpeciality(int id)
        {
            try
            {
                var getData = await _SpecialitiesRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _SpecialitiesRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Speciality deleted successfully" });
                    else
                        return Json(new { success = false, message = "The Speciality cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Speciality not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the Speciality. Please try again." });
            }
        }

        private bool SpecialityExists(int id)
        {
            return _SpecialitiesRepository.GetAll().Any(e => e.Id == id);
        }
    }
}

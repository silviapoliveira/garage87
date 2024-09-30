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

namespace garage87.Controllers
{
    public class ServicesController : Controller
    {

        private readonly IServiceRepository _serviceRepository;
        private readonly IImageHelper _imageHelper;
        public ServicesController(IServiceRepository serviceRepository, IImageHelper imageHelper)
        {
            _serviceRepository = serviceRepository;
            _imageHelper = imageHelper;
        }

        public async Task<IActionResult> Index()
        {
            var services = _serviceRepository.GetAll();
            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceViewModel service)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (service.ImageFile != null && service.ImageFile.Length > 0)
                {
                    path = await _imageHelper.UploadImageAsync(service.ImageFile, "Services");
                }
                service.ImageUrl = path;
                var obj = service.GetEntity(null, path);
                await _serviceRepository.CreateAsync(obj);
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            var service = await _serviceRepository.GetByIdAsync(id);
            var serviceViewModel = ServiceViewModel.FromEntity(service);
            if (service == null)
            {
                return NotFound();
            }
            return View(serviceViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceViewModel service)
        {
            if (id != service.ServiceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var path = service.ImageUrl;

                    if (service.ImageFile != null && service.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(service.ImageFile, "Services");
                    }
                    var obj = await _serviceRepository.GetByIdAsync(id);
                    var data = service.GetEntity(obj, path);
                    await _serviceRepository.UpdateAsync(data);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.ServiceId))
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
            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                var getData = await _serviceRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _serviceRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Service deleted successfully" });
                    else
                        return Json(new { success = false, message = "The Service cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Service not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the Service. Please try again." });
            }
        }

        private bool ServiceExists(int id)
        {
            return _serviceRepository.GetAll().Any(e => e.Id == id);
        }
    }
}

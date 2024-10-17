using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace garage87.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CountriesController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IFlashMessage _flashMessage;

        public CountriesController
            (ICountryRepository countryRepository,
            IFlashMessage flashMessage)
        {
            _countryRepository = countryRepository;
            _flashMessage = flashMessage;
        }
        #region Country

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var countries = _countryRepository.GetAll();
                    bool exists = countries.Any(c => c.Name.ToLower() == country.Name.ToLower() ||
                                    c.CountryCode.ToLower() == country.CountryCode.ToLower());

                    if (exists)
                    {
                        ModelState.AddModelError("Name", "A country with the same name or code already exists.");
                        return View(country);
                    }
                    await _countryRepository.CreateAsync(country);
                    return RedirectToAction(nameof(Index));
                }
                catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627) // SQL error codes for unique constraint violations
                {
                    ModelState.AddModelError("Name", "This country already exists.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred.");
                }
            }

            return View(country);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                var countries = _countryRepository.GetAll().Where(x => x.Id != country.Id);
                bool exists = countries.Any(c => c.Name.ToLower() == country.Name.ToLower() ||
                                c.CountryCode.ToLower() == country.CountryCode.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Name", "A country with the same name or code already exists.");
                    return View(country);
                }
                await _countryRepository.UpdateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        #region  Country API

        public ActionResult DropDown([FromBody] DataManagerRequest dm)
        {
            var Data = _countryRepository.GetAll();
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

        [HttpPost]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            try
            {
                var getData = await _countryRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _countryRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Country deleted successfully" });
                    else
                        return Json(new { success = false, message = "The country cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! country not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the country. Please try again." });
            }
        }

        public IActionResult GetCountriesList([FromBody] DataManagerRequest dm)
        {
            IQueryable<Country> Data = _countryRepository.GetAll();
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

        #endregion

        #region Cities

        public IActionResult Cities()
        {
            return View();
        }

        public IActionResult AddCity()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (model.CountryId <= 0)
            {
                ModelState.AddModelError("CountryId", "Please Select Country.");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var cities = _countryRepository.GetCities();
                bool exists = cities.Any(c => c.Name.ToLower() == model.Name.ToLower() &&
                                c.CountryId == model.CountryId);

                if (exists)
                {
                    ModelState.AddModelError("Name", "A city with the same name and country already exists.");
                    return View(model);
                }
                await _countryRepository.AddCityAsync(model);
                return RedirectToAction("Cities", "Countries");
            }

            return View(model);
        }

        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(City city)
        {
            if (city.CountryId <= 0)
            {
                ModelState.AddModelError("CountryId", "Please select country.");
                return View(city);
            }
            if (ModelState.IsValid)
            {
                var cities = _countryRepository.GetCities().Where(x => x.Id != city.Id);
                bool exists = cities.Any(c => c.Name.ToLower() == city.Name.ToLower() &&
                                c.CountryId == city.CountryId);

                if (exists)
                {
                    ModelState.AddModelError("Name", "A city with the same name and country already exists.");
                    return View(city);
                }
                var countryId = await _countryRepository.UpdateCityAsync(city);
                if (countryId != 0)
                {
                    return RedirectToAction("Cities", "Countries");
                }
            }

            return View(city);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCity(int id)
        {
            try
            {
                var getData = await _countryRepository.GetCityAsync(id);
                if (getData != null)
                {
                    var success = await _countryRepository.DeleteCityAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "City deleted successfully" });
                    else
                        return Json(new { success = false, message = "The city cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! City not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the city. Please try again." });
            }
        }

        #region Cities API

        public ActionResult CitiesDropdown([FromBody] DataManagerRequest dm)
        {
            var Data = _countryRepository.GetCities();
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

        public IActionResult GetCitiesList([FromBody] DataManagerRequest dm)
        {
            IQueryable<City> Data = _countryRepository.GetCities();
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
        #endregion
    }
}

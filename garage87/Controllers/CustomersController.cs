using garage87.Data.Entities;
using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vereyon.Web;

namespace garage87.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly UserManager<User> _userManager;
        public CustomersController
            (ICustomerRepository customerRepository,
            IImageHelper imageHelper,
            IConverterHelper converterHelper,
            IFlashMessage flashMessage,
            IUserHelper userHelper,
            IMailHelper mailHelper,
             UserManager<User> userManager,
             ICountryRepository countryRepository)
        {
            _customerRepository = customerRepository;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _userManager = userManager;
            _countryRepository = countryRepository;

        }
        #region Vehicles



        [Authorize(Roles = "Employee,Mechanic")]
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
        [Authorize(Roles = "Employee,Mechanic")]
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
        [Authorize(Roles = "Employee,Mechanic")]
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
        [Authorize(Roles = "Employee,Mechanic")]
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
        [Authorize(Roles = "Employee,Mechanic")]
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

        #endregion

        #region Customer Crud
        [Authorize(Roles = "Admin,Employee,Mechanic")]
        public IActionResult Index()
        {
            var customer = _customerRepository.GetAll().Include(x => x.City);
            return View(customer);
        }
        [Authorize(Roles = "Admin,Employee,Mechanic")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Employee,Mechanic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.CityId <= 0)
                    {
                        ModelState.AddModelError("CityId", "Please Select City.");
                        return View(model);
                    }
                    var customers = _customerRepository.GetAll();
                    bool exists = customers.Any(c => c.VatNumber == model.VatNumber);

                    if (exists)
                    {
                        ModelState.AddModelError("VatNumber", "A customer with the same VAT Number already exists.");
                        return View(model);
                    }
                    var path = string.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "customers");
                    }

                    var customer = _converterHelper.ToCustomer(model, path, true);
                    var addedByUserid = _userManager.GetUserId(User);
                    var addedByUser = await _userManager.GetUserAsync(User);
                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        CityId = addedByUser.CityId,
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result.Succeeded)
                        await _userManager.AddToRoleAsync(user, "Customer");

                    if (result != IdentityResult.Success)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(model);
                    }

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendEmail(model.Email, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");
                    customer.UserId = user.Id;
                    customer.AddedBy = addedByUserid;

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

        [Authorize(Roles = "Admin,Employee,Mechanic")]
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

        [Authorize(Roles = "Admin,Employee,Mechanic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.CityId <= 0)
                    {
                        ModelState.AddModelError("CityId", "Please Select City.");
                        return View(model);
                    }
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "customers");
                    }
                    var customers = _customerRepository.GetAll();
                    bool exists = customers.Any(c => c.Id != model.Id && c.VatNumber == model.VatNumber);

                    if (exists)
                    {
                        ModelState.AddModelError("VatNumber", "A customer with the same VAT Number already exists.");
                        return View(model);
                    }
                    var customer = _converterHelper.ToCustomer(model, path, false);
                    customer.UserId = model.UserId;
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        // Update existing user details
                        existingUser.FirstName = model.FirstName;
                        existingUser.LastName = model.LastName;
                        existingUser.PhoneNumber = model.PhoneNumber;
                        existingUser.Address = model.Address;

                        var updateResult = await _userManager.UpdateAsync(existingUser);
                        if (updateResult.Succeeded)
                        {

                            await _customerRepository.UpdateAsync(customer);
                        }
                        else
                        {
                            // Add validation errors if update fails
                            foreach (var error in updateResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }
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
        [Authorize(Roles = "Admin,Employee,Mechanic")]
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var getData = await _customerRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var userId = getData.UserId;
                    var success = await _customerRepository.DeleteAsync(getData);
                    if (success == true)
                    {
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null)
                        {
                            var result = await _userManager.DeleteAsync(user);

                        }
                        return Json(new { success = true, message = "Customer deleted successfully" });
                    }
                    else
                        return Json(new { success = false, message = "The Customer cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Customer not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the Customer. Please try again." });
            }
        }
        #region API method
        public IActionResult GetCustomersList([FromBody] DataManagerRequest dm)
        {
            IQueryable<Customer> Data = _customerRepository.GetAll().Include(x => x.City);
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

        public ActionResult Dropdown([FromBody] DataManagerRequest dm)
        {
            var Data = _customerRepository.GetAll();
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
        #endregion
        #endregion
    }
}

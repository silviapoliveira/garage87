using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using garage87.Data;
using garage87.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Vereyon.Web;
using garage87.Models;
using garage87.Helpers;
using Syncfusion.EJ2.Base;
using Microsoft.AspNetCore.Identity;
using garage87.Data.Repositories.IRepository;

namespace garage87.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly UserManager<User> _userManager;
        public EmployeesController
            (IEmployeeRepository employeeRepository,
            IImageHelper imageHelper,
            IConverterHelper converterHelper,
            IFlashMessage flashMessage, IUserHelper userHelper, IMailHelper mailHelper, UserManager<User> userManager, IMessageRepository messageRepository)
        {
            _employeeRepository = employeeRepository;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _userManager = userManager;
            _messageRepository = messageRepository;
        }
        public IActionResult EmployeeDashboard()
        {
            return View();
        }


        #region Employee Crud
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var obj = _employeeRepository.GetAll();
            return View(obj);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = string.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "employees");
                    }

                    var employee = _converterHelper.ToEmployee(model, path, true);

                    var user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.Email,
                        PhoneNumber = model.PhoneNo,
                        CityId = model.CityId,
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result.Succeeded)
                        if (model.Function == (int)Enums.EmployeeFunctionEnum.Mechanic)
                        {
                            await _userManager.AddToRoleAsync(user, "Mechanic");
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, "Employee");
                        }

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
                       $"<strong>Your Password: </strong> {model.Password}" +
                        $"To allow the user, " +
                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");
                    employee.UserId = user.Id;
                    employee.AddedBy = _userManager.GetUserId(User);

                    await _employeeRepository.CreateAsync(employee);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("This employee already exists. Please remember VAT number must be unique.");
                }

                return View(model);
            }

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value, e => e.User);

            if (employee == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEmployeeViewModel(employee);

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var path = model.ImageUrl;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "employees");
                    }

                    var employee = _converterHelper.ToEmployee(model, path, false);
                    employee.UserId = model.UserId;
                    employee.AddedBy = model.AddedBy;
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null)
                    {
                        // Update existing user details
                        existingUser.FirstName = model.FirstName;
                        existingUser.LastName = model.LastName;
                        existingUser.PhoneNumber = model.PhoneNo;
                        existingUser.CityId = model.CityId;

                        var updateResult = await _userManager.UpdateAsync(existingUser);
                        if (updateResult.Succeeded)
                        {
                            await _employeeRepository.UpdateAsync(employee);
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var getData = await _employeeRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _employeeRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Employee deleted successfully" });
                    else
                        return Json(new { success = false, message = "The Employee cannot be deleted because it is associated with other records." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! Employee not deleted" });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the Employee. Please try again." });
            }
        }

        public IActionResult Messages()
        {
            return View();
        }

        #region API method
        public IActionResult GetEmployeesList([FromBody] DataManagerRequest dm)
        {
            IQueryable<Employee> Data = _employeeRepository.GetAll();
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
        public IActionResult GetMessages([FromBody] DataManagerRequest dm)
        {
            IQueryable<Message> Data = _messageRepository.GetAll();
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            try
            {
                var getData = await _messageRepository.GetByIdAsync(id);
                if (getData != null)
                {
                    var success = await _messageRepository.DeleteAsync(getData);
                    if (success == true)
                        return Json(new { success = true, message = "Query deleted successfully" });
                    else
                        return Json(new { success = false, message = "The query cannot be deleted. Try again later." });

                }
                else
                {
                    return Json(new { success = false, message = "Error! The query cannot be deleted. Try again later." });
                }
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "An error occurred while deleting the query. Please try again." });
            }
        }
        #endregion
        #endregion
    }
}

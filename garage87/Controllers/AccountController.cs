

using AspNetCoreHero.ToastNotification.Abstractions;
using garage87.Data.Entities;
using garage87.Data.Repositories;
using garage87.Data.Repositories.IRepository;
using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Syncfusion.EJ2.Base;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace garage87.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly ICountryRepository _countryRepository;
        private readonly UserManager<User> _userManager;
        private readonly INotyfService _notyf;
        private readonly ICustomerRepository _customerRepo;
        private readonly IEmployeeRepository _employeeRepo;
        public AccountController(
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IConfiguration configuration,
            ICountryRepository countryRepository,
            UserManager<User> userManager,
            INotyfService notyf,
            ICustomerRepository customerRepo,
            IEmployeeRepository employeeRepo)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _countryRepository = countryRepository;
            _userManager = userManager;
            _notyf = notyf;
            _customerRepo = customerRepo;
            _employeeRepo = employeeRepo;

        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }
                    var user = await _userHelper.GetUserByEmailAsync(model.Username);
                    bool isAdmin = await _userHelper.IsUserInRoleAsync(user, "Admin");
                    bool isEmployee = await _userHelper.IsUserInRoleAsync(user, "Employee");
                    bool isMechanic = await _userHelper.IsUserInRoleAsync(user, "Mechanic");

                    if (isAdmin)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (isEmployee)
                    {
                        return RedirectToAction("EmployeeDashboard", "Employees");
                    }
                    else if (isMechanic)
                    {
                        return RedirectToAction("Index", "Mechanic");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            var model = new RegisterNewUserViewModel
            {
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(0)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId);

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        CityId = model.CityId,
                        City = city,
                    };

                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        ModelState.AddModelError(string.Empty, "The user couldn't be created.");
                        return View(model);
                    }
                    var customer = new Customer
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        CityId = model.CityId,
                        UserId = user.Id,
                        VatNumber = model.Vat,
                        ZipCode = model.ZipCode,
                        AddedBy = null,
                    };
                    await _customerRepo.CreateAsync(customer);

                    var roleResult = await _userManager.AddToRoleAsync(user, "Customer");
                    if (!roleResult.Succeeded)
                    {
                        foreach (var error in roleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        ModelState.AddModelError(string.Empty, "The user couldn't be assigned to the Customer role.");
                        return View(model);
                    }
                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userId = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendEmail(model.Username, "Email confirmation", $"<h1>Email Confirmation</h1>" +
                        $"To allow the user, " +
                        $"please click in this link:</br></br><a href = \"{tokenLink}\">Confirm Email</a>");

                    if (response.IsSuccess)
                    {
                        _notyf.Success("The instructions to allow user have been sent to the email.");
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "The user couldn't be logged.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
            //var customer = _customerRepo.GetAll().Where(x => x.UserId == user.Id).FirstOrDefault();
            var model = new ChangeUserViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Address = user.Address;
                model.PhoneNumber = user.PhoneNumber;
                model.Email = user.Email;
                if (User.IsInRole("Customer"))
                {
                    var customer = _customerRepo.GetAll().FirstOrDefault(x => x.UserId == user.Id);
                    if (customer != null)
                    {
                        model.Vat = customer.VatNumber;
                        model.ZipCode = customer.ZipCode;
                    }
                }
                // Check if the user is in the "Employee" role
                else if (User.IsInRole("Employee"))
                {
                    var employee = _employeeRepo.GetAll().FirstOrDefault(x => x.UserId == user.Id);
                    if (employee != null)
                    {
                        model.Vat = employee.VatNumber;
                        //model.ZipCode = employee.ZipCode;
                    }
                }
                else if (User.IsInRole("Mechanic"))
                {
                    var employee = _employeeRepo.GetAll().FirstOrDefault(x => x.UserId == user.Id);
                    if (employee != null)
                    {
                        model.Vat = employee.VatNumber;
                        //model.ZipCode = employee.ZipCode;
                    }
                }


                var city = await _countryRepository.GetCityAsync(user.CityId);
                if (city != null)
                {
                    model.CityId = user.CityId;
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var city = await _countryRepository.GetCityAsync(model.CityId);

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;
                    user.CityId = model.CityId;
                    user.City = city;

                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {
                        if (User.IsInRole("Customer"))
                        {
                            var customer = _customerRepo.GetAll().FirstOrDefault(x => x.UserId == user.Id);
                            if (customer != null)
                            {
                                customer.ZipCode = model.ZipCode;
                                customer.VatNumber = model.Vat;
                                customer.FirstName = model.FirstName;
                                customer.LastName = model.LastName;
                                customer.Address = model.Address;
                                customer.CityId = model.CityId;
                                customer.PhoneNumber = model.PhoneNumber;
                                await _customerRepo.UpdateAsync(customer);
                            }
                        }
                        // Check if user is in role "Employee"
                        else if (User.IsInRole("Employee"))
                        {
                            var employee = _employeeRepo.GetAll().FirstOrDefault(x => x.UserId == user.Id);
                            if (employee != null)
                            {
                                employee.VatNumber = model.Vat;
                                employee.FirstName = model.FirstName;
                                employee.LastName = model.LastName;
                                await _employeeRepo.UpdateAsync(employee);
                            }
                        }
                        else if (User.IsInRole("Mechanic"))
                        {
                            var employee = _employeeRepo.GetAll().FirstOrDefault(x => x.UserId == user.Id);
                            if (employee != null)
                            {
                                employee.VatNumber = model.Vat;
                                employee.FirstName = model.FirstName;
                                employee.LastName = model.LastName;
                                await _employeeRepo.UpdateAsync(employee);
                            }
                        }
                        _notyf.Success("User updated successfully!");
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (this.User.Identity.Name == null)
                {
                    _notyf.Warning("Please login first to change your password!");
                    return View(model);
                }
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _configuration["Tokens:Issuer"],
                            _configuration["Tokens:Audience"],
                            claims,
                            expires: DateTime.UtcNow.AddDays(15),
                            signingCredentials: credentials);
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            // Check the user's role after successful email confirmation
            var isEmployee = await _userHelper.IsUserInRoleAsync(user, "Employee");
            var isMechanic = await _userHelper.IsUserInRoleAsync(user, "Mechanic");
            var customer = _customerRepo.GetAll().Where(x => x.UserId == user.Id).FirstOrDefault();
            if (customer != null)
            {
                if (customer.AddedBy != null)
                {
                    var isCustomer = await _userHelper.IsUserInRoleAsync(user, "Customer");
                    ViewBag.IsCustomer = isCustomer;
                }
            }
            // Pass the role information to the view
            ViewBag.IsEmployee = isEmployee;
            ViewBag.IsMechanic = isMechanic;

            if (!result.Succeeded)
            {

            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email does not correspond to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                var link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken },
                    protocol: HttpContext.Request.Scheme);

                // Send email with reset link
                var response = _mailHelper.SendEmail(
                    model.Email,
                    "Shop Password Reset",
                    $"<h1>Shop Password Reset</h1>" +
                    $"To reset your password, click on this link:</br></br>" +
                    $"<a href=\"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                {
                    _notyf.Success("Instructions to recover your password have been sent to your email.");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    _notyf.Error("There was an issue sending the recovery email. Please try again.");
                    return View(model);
                }
            }
            return View(model);
        }


        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.Username);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    _notyf.Success("Password reset successful.");
                    return RedirectToAction("Login", "Account");
                }

                _notyf.Error("Error while resetting the password.");
                return View(model);
            }
            _notyf.Error("User not found.");
            return View(model);
        }

        public IActionResult NotAuthorized()
        {
            return View();
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
    }

}

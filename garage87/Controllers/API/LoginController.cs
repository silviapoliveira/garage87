using garage87.Helpers;
using garage87.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace garage87.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IUserHelper _userHelper;
        public LoginController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);

                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                bool isAdmin = await _userHelper.IsUserInRoleAsync(user, "Admin");
                bool isEmployee = await _userHelper.IsUserInRoleAsync(user, "Employee");
                bool isMechanic = await _userHelper.IsUserInRoleAsync(user, "Mechanic");

                if (isAdmin)
                {
                    return Ok(new
                    {
                        Success = true,
                        Role = "Admin"
                    });
                }
                else if (isEmployee)
                {
                    return Ok(new
                    {
                        Success = true,
                        Role = "Employee"
                    });
                }
                else if (isMechanic)
                {
                    return Ok(new
                    {
                        Success = true,
                        Role = "Mechanic"
                    });
                }

                return NotFound(model);
            }

            return UnprocessableEntity(model);
        }
    }
}

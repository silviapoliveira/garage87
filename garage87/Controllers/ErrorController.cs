using Microsoft.AspNetCore.Mvc;

namespace garage87.Controllers
{
    public class ErrorController : Controller
    {
        [Route("error/{statusCode}")]
        [HttpGet]
        public IActionResult HandleErrorCode(int statusCode)
        {
            ViewData["StatusCode"] = statusCode;

            string message = statusCode switch
            {
                404 => "Sorry, the page you are looking for cannot be found.",
                500 => "An unexpected error occurred on the server.",
                403 => "You do not have permission to access this resource.",
                _ => "An error occurred. Please try again later."
            };

            ViewData["ErrorMessage"] = message;
            return View("Error");
        }
    }
}

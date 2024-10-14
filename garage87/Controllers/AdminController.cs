using Microsoft.AspNetCore.Mvc;

namespace garage87.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

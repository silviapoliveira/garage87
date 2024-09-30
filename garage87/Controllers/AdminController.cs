using garage87.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

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

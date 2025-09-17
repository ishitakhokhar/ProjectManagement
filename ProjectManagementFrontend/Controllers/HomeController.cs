using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementrontend.Models;

namespace ProjectManagementrontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

           [HttpPost]
    public IActionResult Index(string selectedRole)
    {
        if (string.IsNullOrEmpty(selectedRole))
        {
            ViewBag.Error = "Please select a role!";
            return View();
        }

        // Store role in session
        HttpContext.Session.SetString("UserRole", selectedRole);

            // Redirect to Dashboard
            return RedirectToAction("Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

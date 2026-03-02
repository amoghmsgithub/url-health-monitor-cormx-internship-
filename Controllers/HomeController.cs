using Microsoft.AspNetCore.Mvc;

namespace UrlHealthMonitor.Controllers
{
    public class HomeController : Controller
    {
        // HOME PAGE
        public IActionResult Index()
        {
            return View();
        }

        // ✅ USER MANUAL (THIS IS THE REAL ONE)
        public IActionResult UserManual()
        {
            return View();
        }

        // PRIVACY
        public IActionResult Privacy()
        {
            return View();
        }
    }
}

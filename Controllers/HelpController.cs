using Microsoft.AspNetCore.Mvc;

namespace UrlHealthMonitor.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

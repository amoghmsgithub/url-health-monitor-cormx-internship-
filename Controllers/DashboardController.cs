using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlHealthMonitor.Data;
using System.Linq;

namespace UrlHealthMonitor.Controllers
{
    [Authorize(Roles = "Admin,Viewer,SuperAdmin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var total = _context.MonitoredUrls.Count();
            var up = _context.MonitoredUrls.Count(x => x.DownSince == null);
            var down = _context.MonitoredUrls.Count(x => x.DownSince != null);

            ViewBag.Total = total;
            ViewBag.Up = up;
            ViewBag.Down = down;
            ViewBag.Availability = total == 0 ? 0 : (up * 100 / total);

            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;
using UrlHealthMonitor.Services;

namespace UrlHealthMonitor.Controllers
{
    // 🔒 ADMIN ONLY ACCESS
    [Authorize(Roles = "Admin,Viewer,SuperAdmin")]
    public class MonitoredUrlsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly RecoveryClientService _recoveryClient;

        public MonitoredUrlsController(
            AppDbContext context,
            RecoveryClientService recoveryClient)
        {
            _context = context;
            _recoveryClient = recoveryClient;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var urls = await _context.MonitoredUrls
                .Include(u => u.Group)
                .ToListAsync();

            return View(urls);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var url = await _context.MonitoredUrls
                .Include(u => u.Group)
                .Include(u => u.UrlOutages)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (url == null)
                return NotFound();

            return View(url);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create()
        {
            ViewBag.Groups = _context.Groups.ToList();
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonitoredUrl model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Groups = _context.Groups.ToList();
                return View(model);
            }

            model.HealthStatus = "Unknown";
            model.LastUpdated = DateTime.UtcNow;

            _context.MonitoredUrls.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var url = await _context.MonitoredUrls.FindAsync(id);
            if (url == null)
                return NotFound();

            ViewBag.Groups = _context.Groups.ToList();
            return View(url);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MonitoredUrl model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Groups = _context.Groups.ToList();
                return View(model);
            }

            var existing = await _context.MonitoredUrls
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (existing == null)
                return NotFound();

            // preserve runtime fields
            model.GroupId = existing.GroupId;
            model.HealthStatus = existing.HealthStatus;
            model.DownSince = existing.DownSince;
            model.LastRecoveryRequestId = existing.LastRecoveryRequestId;
            model.LastRecoveryStatus = existing.LastRecoveryStatus;
            model.LastRecoveryAt = existing.LastRecoveryAt;

            model.LastUpdated = DateTime.UtcNow;

            _context.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        public async Task<IActionResult> Delete(int id)
        {
            var url = await _context.MonitoredUrls
                .Include(u => u.Group)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (url == null)
                return NotFound();

            return View(url);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var url = await _context.MonitoredUrls.FindAsync(id);

            if (url != null)
            {
                _context.MonitoredUrls.Remove(url);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // 🔁 RECOVER (MANUAL)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Recover(int id)
        {
            var endpoint = await _context.MonitoredUrls.FindAsync(id);
            if (endpoint == null)
                return NotFound();

            var ack = await _recoveryClient.TriggerRecoveryAsync(endpoint);

            if (ack != null && ack.Ack)
            {
                endpoint.LastRecoveryRequestId = ack.RequestId;
                endpoint.LastRecoveryStatus = "Pending";
                endpoint.LastRecoveryAt = DateTime.UtcNow;

                _context.Update(endpoint);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Recovery request acknowledged";
            }
            else
            {
                TempData["Error"] = "Recovery request failed";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Services;

namespace UrlHealthMonitor.Controllers
{
    // 🔒 ADMIN ONLY ACCESS
    [Authorize(Roles = "Admin,Viewer,SuperAdmin")]
    public class ReportsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ReportPdfService _pdfService;

        public ReportsController(AppDbContext context, ReportPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index()
        {
            var outages = await _context.UrlOutages
                .Include(o => o.MonitoredUrl)
                .OrderByDescending(o => o.DownStartedAt)
                .ToListAsync();

            return View(outages);
        }

        // =========================
        // DOWNLOAD PDF
        // =========================
        public async Task<IActionResult> DownloadPdf()
        {
            var outages = await _context.UrlOutages
                .Include(o => o.MonitoredUrl)
                .OrderByDescending(o => o.DownStartedAt)
                .ToListAsync();

            if (!outages.Any())
                return RedirectToAction(nameof(Index));

            var pdfBytes = _pdfService.GeneratePdf(outages);

            return File(
                pdfBytes,
                "application/pdf",
                $"Outage_Report_{DateTime.UtcNow:yyyyMMdd_HHmm}.pdf"
            );
        }
    }
}

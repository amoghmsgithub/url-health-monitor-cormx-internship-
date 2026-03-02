using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Data;

namespace UrlHealthMonitor.Controllers.Api
{
    [ApiController]
    [Route("api/url-status")]
    public class UrlStatusApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UrlStatusApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            var urls = await _context.MonitoredUrls
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Url,
                    Health = u.HealthStatus,   // ✅ FIXED
                    u.LastCheckTime,
                    u.DownSince
                })
                .ToListAsync();

            return Ok(urls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatusById(int id)
        {
            var url = await _context.MonitoredUrls.FindAsync(id);
            if (url == null)
                return NotFound();

            return Ok(new
            {
                url.Id,
                url.Name,
                url.Url,
                Health = url.HealthStatus,   // ✅ FIXED
                url.LastCheckTime,
                url.DownSince
            });
        }
    }
}




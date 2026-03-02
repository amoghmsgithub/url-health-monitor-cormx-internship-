using Microsoft.AspNetCore.Mvc;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Controllers
{
    [ApiController]
    [Route("api/recovery")]
    public class RecoveryCallbackController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RecoveryCallbackController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] RecoveryCallbackDto dto)
        {
            var url = _context.MonitoredUrls
                .FirstOrDefault(u => u.RecoveryStatus == "Pending"); 

            if (url == null)
                return NotFound("No pending recovery found");
          
            url.RecoveryStatus = dto.Status;
            url.RecoveryMessage = dto.Message;
            url.RecoveryCompletedAt = DateTime.UtcNow;

            // ✅ If recovery success → mark healthy
            if (dto.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
            {
                url.HealthStatus = "Healthy";
                url.DownSince = null;
            }

            await _context.SaveChangesAsync();

            return Ok(new { ack = true });
        }
    }
}

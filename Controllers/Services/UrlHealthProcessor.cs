using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Services
{
    public class UrlHealthProcessor : IUrlHealthProcessor
    {
        private readonly AppDbContext _db;
        private readonly IEmailService _emailService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<UrlHealthProcessor> _logger;

        public UrlHealthProcessor(
            AppDbContext db,
            IEmailService emailService,
            HttpClient httpClient,
            ILogger<UrlHealthProcessor> logger)
        {
            _db = db;
            _emailService = emailService;
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task ProcessAsync(CancellationToken cancellationToken)
        {
            var urls = await _db.MonitoredUrls.ToListAsync(cancellationToken);

            foreach (var url in urls)
            {
                bool isHealthyNow;

                try
                {
                    var response = await _httpClient.GetAsync(url.Url, cancellationToken);
                    isHealthyNow = response.IsSuccessStatusCode;
                }
                catch
                {
                    isHealthyNow = false;
                }

                // ✅ ADDED FIX: Update status when it is Unknown or changes
                if (isHealthyNow)
                {
                    url.HealthStatus = "Healthy";
                }
                else
                {
                    url.HealthStatus = "Down";
                }

                if (url.HealthStatus == "Healthy" && !isHealthyNow)
                {
                    url.HealthStatus = "Down";
                    await _emailService.SendDownAlertAsync(url);
                }

                url.LastCheckTime = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Services;

namespace UrlHealthMonitor.Services
{
    public class WeeklyReportEmailService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<WeeklyReportEmailService> _logger;

        private DateTime _lastRunUtc = DateTime.MinValue;

        public WeeklyReportEmailService(
            IServiceScopeFactory scopeFactory,
            ILogger<WeeklyReportEmailService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Run once every 7 days
                    if ((DateTime.UtcNow - _lastRunUtc).TotalDays < 7)
                    {
                        await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var pdfService = scope.ServiceProvider.GetRequiredService<ReportPdfService>();

                    // Get URLs that have alert emails
                    var urlsWithEmails = await db.MonitoredUrls
                        .Where(u => !string.IsNullOrWhiteSpace(u.AlertEmails))
                        .ToListAsync(stoppingToken);

                    if (!urlsWithEmails.Any())
                    {
                        _lastRunUtc = DateTime.UtcNow;
                        continue;
                    }

                    // Get outages (same logic as before groups)
                    var outages = await db.UrlOutages
                        .Include(o => o.MonitoredUrl)
                        .ToListAsync(stoppingToken);

                    if (!outages.Any())
                    {
                        _lastRunUtc = DateTime.UtcNow;
                        continue;
                    }

                    // ✅ CORRECT METHOD (OLD STYLE REPORT)
                    var pdfBytes = pdfService.GeneratePdf(outages);

                    var allEmails = string.Join(",",
                        urlsWithEmails
                            .Select(u => u.AlertEmails)
                            .Where(e => !string.IsNullOrWhiteSpace(e))
                    );

                    var subject = "📊 Weekly URL Health Report";
                    var body = @"
                        <h3>Weekly URL Health Report</h3>
                        <p>Please find the attached summary availability report.</p>
                    ";

                    await emailService.SendEmailAsync(
                        allEmails,
                        subject,
                        body,
                        pdfBytes,
                        $"UrlHealthReport_{DateTime.UtcNow:yyyyMMdd}.pdf"
                    );

                    _lastRunUtc = DateTime.UtcNow;
                    _logger.LogInformation("Weekly URL health report email sent");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Weekly report email failed");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}

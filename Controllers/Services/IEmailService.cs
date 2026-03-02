using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmails,
            string subject,
            string body,
            byte[]? attachmentBytes = null,
            string? attachmentName = null
        );

        // 🔔 Called when a monitored URL goes DOWN
        Task SendDownAlertAsync(MonitoredUrl url);
    }
}

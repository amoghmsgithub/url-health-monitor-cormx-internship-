using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string toEmails,
            string subject,
            string body,
            byte[]? attachmentBytes = null,
            string? attachmentName = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmails))
                    return;

                var smtp = _configuration.GetSection("SmtpSettings");

                using var client = new SmtpClient
                {
                    Host = smtp["Host"],
                    Port = int.Parse(smtp["Port"]),
                    EnableSsl = bool.Parse(smtp["EnableSsl"]),
                    Credentials = new NetworkCredential(
                        smtp["Username"],
                        smtp["Password"]
                    )
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(smtp["From"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                foreach (var email in toEmails.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    message.To.Add(email.Trim());
                }

                // 📎 Attachment support
                if (attachmentBytes != null && !string.IsNullOrWhiteSpace(attachmentName))
                {
                    var stream = new MemoryStream(attachmentBytes);
                    message.Attachments.Add(
                        new Attachment(stream, attachmentName, "application/pdf")
                    );
                }

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent to {Emails}", toEmails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email sending failed");
            }
        }

        // 🔔 URL DOWN ALERT
        public async Task SendDownAlertAsync(MonitoredUrl url)
        {
            var alertEmails = _configuration["AlertSettings:AlertEmails"];

            if (string.IsNullOrWhiteSpace(alertEmails))
            {
                _logger.LogWarning("No alert emails configured");
                return;
            }

            var subject = $"🚨 CORMX ALERT: {url.Name} is DOWN";

            var body = $@"
<h3>URL Down Alert</h3>
<p><strong>Name:</strong> {url.Name}</p>
<p><strong>URL:</strong> {url.Url}</p>
<p><strong>Status:</strong> DOWN</p>
<p><strong>Detected At (UTC):</strong> {DateTime.UtcNow}</p>
<p><strong>Group:</strong> {url.Group?.Name ?? "N/A"}</p>
<br/>
<p>– CORMX Monitoring</p>
";

            await SendEmailAsync(alertEmails, subject, body);
        }
    }
}

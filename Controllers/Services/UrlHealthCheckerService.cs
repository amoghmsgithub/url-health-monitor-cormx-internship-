using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrlHealthMonitor.Services
{
    public class UrlHealthCheckerService : BackgroundService
    {
        private readonly ILogger<UrlHealthCheckerService> _logger;
        private readonly IUrlHealthProcessor _processor;

        public UrlHealthCheckerService(
            ILogger<UrlHealthCheckerService> logger,
            IUrlHealthProcessor processor)
        {
            _logger = logger;
            _processor = processor;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("UrlHealthCheckerService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _processor.ProcessAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing URL health checks.");
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }

            _logger.LogInformation("UrlHealthCheckerService stopping.");
        }
    }
}
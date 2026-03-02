using System;

namespace UrlHealthMonitor.Models
{
    public class UrlOutage
    {
        public int Id { get; set; }

        public int MonitoredUrlId { get; set; }
        public MonitoredUrl MonitoredUrl { get; set; } = null!;

        public DateTime DownStartedAt { get; set; }

        public DateTime? RecoveredAt { get; set; }

        // 🔴 THIS WAS MISSING (CAUSE OF BUILD FAILURE)
        public bool IsResolved { get; set; }

        public double? DurationSeconds { get; set; }
    }
}

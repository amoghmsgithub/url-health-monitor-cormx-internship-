using System;
using System.Collections.Generic;

namespace UrlHealthMonitor.Models
{
    public class MonitoredUrl
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public int IntervalSeconds { get; set; }

        // ✅ SINGLE SOURCE OF TRUTH
        public string HealthStatus { get; set; } = "Unknown";

        public DateTime LastUpdated { get; set; }
        public DateTime? DownSince { get; set; }

        public DateTime? LastCheckTime { get; set; }
        public int? ResponseTimeMs { get; set; }

        // 🔔 Alerts
        public string? AlertEmails { get; set; }
        public bool SendDownAlert { get; set; }

        // ===============================
        // GROUP RELATION
        // ===============================
        public int GroupId { get; set; }
        public Group? Group { get; set; }

        // ===============================
        // OUTAGES
        // ===============================
        public List<UrlOutage> UrlOutages { get; set; } = new();

        // ===============================
        // 🔁 RECOVERY CONFIG
        // ===============================
        public string? AppPoolName { get; set; }
        public string? RecoveryWebsiteUrl { get; set; }
        public int? RecoveryAction { get; set; }

        // ===============================
        // 🧠 RECOVERY STATE (NEW – ACK TRACKING)
        // ===============================

        // RequestId returned by recovery server (ACK)
        public string? LastRecoveryRequestId { get; set; }

        // Pending / Success / Failed
        public string? LastRecoveryStatus { get; set; }

        // When recovery was triggered
        public DateTime? LastRecoveryAt { get; set; }
        // ===============================

// 🔔 RECOVERY CALLBACK STATUS
// ===============================
public string? RecoveryStatus { get; set; }   // Pending / Success / Failed
public string? RecoveryMessage { get; set; }
public DateTime? RecoveryCompletedAt { get; set; }

    }
}

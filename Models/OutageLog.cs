using System;

namespace CORMX.Models
{
    public class OutageLog
    {
        public int Id { get; set; }

        public string UrlName { get; set; }

        public string Url { get; set; }

        public string Status { get; set; }   // DOWN / RECOVERED

        public DateTime LastChecked { get; set; }

        public int? ResponseTimeMs { get; set; }

        public int DurationSeconds { get; set; }
    }
}

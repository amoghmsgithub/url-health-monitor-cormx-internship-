namespace UrlHealthMonitor.Models
{
    public class RecoveryCallbackDto
    {
        public string RequestId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Success / Failed
        public string? Message { get; set; }
    }
}

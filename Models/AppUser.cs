using System.Collections.Generic;

namespace UrlHealthMonitor.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        /* -------------------------------
           RBAC
           Each User → One Role
           ------------------------------- */
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        /* -------------------------------
           EXISTING RELATIONS (if any)
           ------------------------------- */
        public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
        public ICollection<MonitoredUrl> MonitoredUrls { get; set; } = new List<MonitoredUrl>();
    }
}

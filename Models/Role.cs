using System.Collections.Generic;

namespace UrlHealthMonitor.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();
    }
}

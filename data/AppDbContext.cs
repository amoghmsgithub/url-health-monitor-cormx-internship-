using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Role> Roles { get; set; }   // RBAC
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<MonitoredUrl> MonitoredUrls { get; set; }
        public DbSet<UrlOutage> UrlOutages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* -------------------------------
               USER ↔ ROLE (RBAC)
               One Role → Many Users
               ------------------------------- */
            modelBuilder.Entity<Role>()
                .HasMany(r => r.AppUsers)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            /* -------------------------------
               ROLE SEEDING
               ------------------------------- */
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" },
                new Role { Id = 3, Name = "Viewer" }
            );

            /* -------------------------------
               USER ↔ GROUP (COMPOSITE KEY)
               ------------------------------- */
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => new { ug.AppUserId, ug.GroupId });

            /* -------------------------------
               GROUP ↔ MONITORED URL  ✅ STEP 3
               One Group → Many URLs
               ------------------------------- */
            modelBuilder.Entity<MonitoredUrl>()
                .HasOne(u => u.Group)
                .WithMany()
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            /* -------------------------------
               MONITORED URL ↔ OUTAGES
               One URL → Many Outages
               ------------------------------- */
            modelBuilder.Entity<UrlOutage>()
                .HasOne(o => o.MonitoredUrl)
                .WithMany(u => u.UrlOutages)
                .HasForeignKey(o => o.MonitoredUrlId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


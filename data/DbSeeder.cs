using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext db)
        {
            // Ensure roles exist (safety check)
            if (!await db.Roles.AnyAsync())
                return;

            await SeedUserIfNotExists(db,
                email: "admin@cormx.com",
                password: "Admin@123",
                roleName: "Admin");

            await SeedUserIfNotExists(db,
                email: "user@cormx.com",
                password: "User@123",
                roleName: "User");

            await SeedUserIfNotExists(db,
                email: "viewer@cormx.com",
                password: "Viewer@123",
                roleName: "Viewer");
        }

        private static async Task SeedUserIfNotExists(
            AppDbContext db,
            string email,
            string password,
            string roleName)
        {
            if (await db.AppUsers.AnyAsync(u => u.Email == email))
                return;

            var role = await db.Roles.FirstAsync(r => r.Name == roleName);

            var user = new AppUser
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RoleId = role.Id
            };

            db.AppUsers.Add(user);
            await db.SaveChangesAsync();
        }
    }
}


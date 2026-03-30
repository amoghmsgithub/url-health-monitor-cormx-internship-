using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(AppDbContext db)
        {
            // -----------------------------
            // ✅ ENSURE EACH ROLE EXISTS
            // -----------------------------
            await EnsureRoleExists(db, "SuperAdmin");
            await EnsureRoleExists(db, "Admin");
            await EnsureRoleExists(db, "Viewer");

            // -----------------------------
            // SUPER ADMIN
            // -----------------------------
            await SeedUserIfNotExists(db,
                email: "superadmin@cormx.com",
                password: "Admin@cormx",
                roleName: "SuperAdmin");

            // -----------------------------
            // ADMIN
            // -----------------------------
            await SeedUserIfNotExists(db,
                email: "admin@cormx.com",
                password: "Admin@123",
                roleName: "Admin");

            // -----------------------------
            // VIEWER
            // -----------------------------
            await SeedUserIfNotExists(db,
                email: "viewer@cormx.com",
                password: "Viewer@123",
                roleName: "Viewer");
        }

        // ✅ NEW METHOD
        private static async Task EnsureRoleExists(AppDbContext db, string roleName)
        {
            if (!await db.Roles.AnyAsync(r => r.Name == roleName))
            {
                db.Roles.Add(new Role { Name = roleName });
                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedUserIfNotExists(
            AppDbContext db,
            string email,
            string password,
            string roleName)
        {
            if (await db.AppUsers.AnyAsync(u => u.Email == email))
                return;

            var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
                throw new Exception($"Role '{roleName}' not found in database.");

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
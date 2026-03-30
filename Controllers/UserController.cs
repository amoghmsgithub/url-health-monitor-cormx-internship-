using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;

namespace UrlHealthMonitor.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // ================= LIST USERS =================
        public async Task<IActionResult> Index()
        {
            var users = await _context.AppUsers
                .Include(u => u.Role)
                .ToListAsync();

            return View(users);
        }

        // ================= CREATE USER (GET) =================
        public async Task<IActionResult> Create()
        {
            var roles = await _context.Roles
                .Where(r => r.Name != "SuperAdmin")
                .ToListAsync();

            ViewBag.Roles = roles;
            return View();
        }

        // ================= CREATE USER (POST) =================
        [HttpPost]
        public async Task<IActionResult> Create(AppUser user, string password)
        {
            if (string.IsNullOrEmpty(user.Email) ||
                string.IsNullOrEmpty(password) ||
                user.RoleId == 0)
            {
                var roles = await _context.Roles
                    .Where(r => r.Name != "SuperAdmin")
                    .ToListAsync();

                ViewBag.Roles = roles;
                return View(user);
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == user.RoleId);

            if (role == null || role.Name == "SuperAdmin")
            {
                var roles = await _context.Roles
                    .Where(r => r.Name != "SuperAdmin")
                    .ToListAsync();

                ViewBag.Roles = roles;
                return View(user);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);

            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.AppUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        // ================= EDIT (GET) =================
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _context.Roles
                .Where(r => r.Name != "SuperAdmin")
                .ToListAsync();

            ViewBag.Roles = roles;

            return View(user);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        public async Task<IActionResult> Edit(int id, AppUser updatedUser)
        {
            if (id != updatedUser.Id)
                return NotFound();

            var user = await _context.AppUsers.FindAsync(id);

            if (user == null)
                return NotFound();

            user.Email = updatedUser.Email;
            user.RoleId = updatedUser.RoleId;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ================= DELETE =================
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);

            if (user == null)
                return NotFound();

            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
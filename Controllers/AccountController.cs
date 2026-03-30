using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models.ViewModels;

namespace UrlHealthMonitor.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        // ================= LOGIN (GET) =================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ================= LOGIN (POST) =================
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _db.AppUsers
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            // 🔑 CLAIMS
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );

            // 🔀 ROLE-BASED REDIRECT
            if (user.Role.Name == "Viewer")
                return RedirectToAction("Index", "Home");

            if (user.Role.Name == "SuperAdmin")
                return RedirectToAction("Index", "Dashboard"); // same for now

            // Admin
            return RedirectToAction("Index", "Dashboard");
        }

        // ================= LOGOUT =================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login");
        }

        // ================= ACCESS DENIED =================
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
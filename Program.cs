using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;
using UrlHealthMonitor.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// AUTH
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// SERVICES
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ReportPdfService>();
builder.Services.AddHttpClient<RecoveryClientService>();

// ✅ NEW — URL HEALTH PROCESSOR REGISTRATION
builder.Services.AddScoped<IUrlHealthProcessor, UrlHealthProcessor>();
builder.Services.AddHttpClient<UrlHealthProcessor>();

// BACKGROUND SERVICES
builder.Services.AddHostedService<UrlHealthCheckerService>();
builder.Services.AddHostedService<WeeklyReportEmailService>();

var app = builder.Build();

// DB MIGRATION + SEED
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedAdminAsync(db);

    if (!db.Companies.Any())
    {
        var company = new Company { Name = "Default Company" };
        db.Companies.Add(company);
        await db.SaveChangesAsync();

        db.Groups.AddRange(
            new Group { Name = "General", CompanyId = company.Id },
            new Group { Name = "HDFC", CompanyId = company.Id },
            new Group { Name = "Yogawawa", CompanyId = company.Id }
        );

        await db.SaveChangesAsync();
    }
}

// PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;
using UrlHealthMonitor.Services;
using Xunit;

namespace UrlHealthMonitor.Tests
{
    public class UrlHealthIntegrationTests
    {
        [Fact]
        public async Task UrlHealthProcessor_ShouldCheckUrlStatus()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("UrlCheckTestDb"));

            services.AddLogging();
            services.AddHttpClient();

            services.AddScoped<IEmailService, FakeEmailService>();
            services.AddScoped<IUrlHealthProcessor, UrlHealthProcessor>();

            var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.MonitoredUrls.Add(new MonitoredUrl
            {
                Url = "https://www.google.com",
                Name = "Google Test"
            });

            await context.SaveChangesAsync();

            var processor = scope.ServiceProvider.GetRequiredService<IUrlHealthProcessor>();

            await processor.ProcessAsync(CancellationToken.None);

            var urls = await context.MonitoredUrls.ToListAsync();

            Assert.NotEmpty(urls);
        }
    }
}

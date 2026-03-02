using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;
using UrlHealthMonitor.Services;
using Xunit;

public class UrlHealthProcessorTests
{
    private AppDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private HttpClient GetFailingHttpClient()
    {
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task ProcessAsync_ShouldMarkUrlDown_AndSendEmail_WhenHttpFails()
    {
        // Arrange
        var db = GetInMemoryDb();

        var monitoredUrl = new MonitoredUrl
        {
            Id = 1,
            Url = "https://example.com",
            HealthStatus = "Healthy"
        };

        db.MonitoredUrls.Add(monitoredUrl);
        await db.SaveChangesAsync();

        var emailMock = new Mock<IEmailService>();

        var httpClient = GetFailingHttpClient();

        var processor = new UrlHealthProcessor(
            db,
            emailMock.Object,
            httpClient,
            NullLogger<UrlHealthProcessor>.Instance);

        // Act
        await processor.ProcessAsync(CancellationToken.None);

        // Assert
        var updatedUrl = await db.MonitoredUrls.FirstAsync();

        Assert.Equal("Down", updatedUrl.HealthStatus);

        emailMock.Verify(
            x => x.SendDownAlertAsync(It.IsAny<MonitoredUrl>()),
            Times.Once);
    }
}
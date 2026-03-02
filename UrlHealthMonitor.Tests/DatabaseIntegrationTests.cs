using Microsoft.EntityFrameworkCore;
using UrlHealthMonitor.Data;
using UrlHealthMonitor.Models;
using Xunit;

public class DatabaseIntegrationTests
{
    [Fact]
    public void CanInsertAndRetrieveCompany()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new AppDbContext(options);

        var company = new Company { Name = "TestCorp" };
        context.Companies.Add(company);
        context.SaveChanges();

        var savedCompany = context.Companies.FirstOrDefaultAsync().Result;

        Assert.NotNull(savedCompany);
        Assert.Equal("TestCorp", savedCompany.Name);
    }
}
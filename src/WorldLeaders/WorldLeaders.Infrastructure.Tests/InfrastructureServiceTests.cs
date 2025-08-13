using WorldLeaders.Infrastructure.Tests.Infrastructure;
using Xunit.Abstractions;

namespace WorldLeaders.Infrastructure.Tests;

/// <summary>
/// Infrastructure service testing validation
/// Context: Educational game infrastructure testing
/// </summary>
public class InfrastructureServiceTests : ServiceTestBase
{
    public InfrastructureServiceTests(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public void ServiceTestBase_ShouldProvideDbContext()
    {
        // Act
        using var context = CreateTestDbContext();
        
        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Database);
    }

    [Fact]
    public async Task ServiceTestBase_ShouldSupportEducationalValidation()
    {
        // Arrange
        var testService = () => Task.FromResult("Learning about geography and countries is exciting!");
        
        // Act & Assert - Should not throw
        await ValidateServiceChildSafety(testService, "Educational Content Service");
        
        // Assert
        Assert.True(true, "Educational service validation should pass for child-safe content");
    }

    [Fact]
    public void ServiceTestBase_ShouldCreateEducationalMocks()
    {
        // Act
        var mockService = CreateEducationalMock<ITestEducationalService>();
        
        // Assert
        Assert.NotNull(mockService);
        Assert.NotNull(mockService.Object);
    }
}

// Test interface for mock creation
public interface ITestEducationalService
{
    Task<string> GetEducationalContentAsync();
}
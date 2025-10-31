using Microsoft.Extensions.DependencyInjection;
using Bugget.DA.Postgres;
using Bugget.Entities.DTO.Report;
using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[Collection("PostgresCollection")]
public class ReportsDbClientTests : IClassFixture<AppWithPostgresFixture>
{
    private readonly ReportsDbClient _reportsDbClient;

    public ReportsDbClientTests(AppWithPostgresFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        _reportsDbClient = scope.ServiceProvider.GetRequiredService<ReportsDbClient>();
    }

    [Fact(DisplayName = "Успешное создание репорта с минимальными параметрами")]
    public async Task CreateReportAsync_WithMinimalParameters_ShouldCreateReport()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var dto = new ReportCreateDto
        {
            Title = "Test Report"
        };

        // Act
        var result = await _reportsDbClient.CreateReportAsync(userId, null, null, dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.Equal(userId, result.ResponsibleUserId);
        Assert.True(result.CreatedAt > DateTimeOffset.MinValue);
        Assert.True(result.UpdatedAt > DateTimeOffset.MinValue);
        Assert.Equal(0, result.Status); // Backlog status
    }

    [Fact(DisplayName = "Успешное создание репорта с teamId")]
    public async Task CreateReportAsync_WithTeamId_ShouldCreateReportWithTeam()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var teamId = $"team_{Guid.NewGuid()}";
        var dto = new ReportCreateDto
        {
            Title = "Test Report with Team"
        };

        // Act
        var result = await _reportsDbClient.CreateReportAsync(userId, teamId, null, dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.Equal(teamId, result.CreatorTeamId);
        Assert.True(result.CreatedAt > DateTimeOffset.MinValue);
    }

    [Fact(DisplayName = "Успешное создание репорта с organizationId")]
    public async Task CreateReportAsync_WithOrganizationId_ShouldCreateReportWithOrganization()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var dto = new ReportCreateDto
        {
            Title = "Test Report with Organization"
        };

        // Act
        var result = await _reportsDbClient.CreateReportAsync(userId, null, organizationId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.True(result.CreatedAt > DateTimeOffset.MinValue);
    }

    [Fact(DisplayName = "Успешное создание репорта со всеми параметрами")]
    public async Task CreateReportAsync_WithAllParameters_ShouldCreateCompleteReport()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var teamId = $"team_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var dto = new ReportCreateDto
        {
            Title = "Complete Test Report"
        };

        // Act
        var result = await _reportsDbClient.CreateReportAsync(userId, teamId, organizationId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.Equal(teamId, result.CreatorTeamId);
        Assert.Equal(userId, result.ResponsibleUserId);
        Assert.True(result.CreatedAt > DateTimeOffset.MinValue);
        Assert.True(result.UpdatedAt > DateTimeOffset.MinValue);
        Assert.Equal(0, result.Status); // Backlog status
    }

    [Fact(DisplayName = "Создание нескольких репортов одним пользователем")]
    public async Task CreateReportAsync_MultipleReports_ShouldCreateSeparateReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var dto1 = new ReportCreateDto { Title = "First Report" };
        var dto2 = new ReportCreateDto { Title = "Second Report" };

        // Act
        var result1 = await _reportsDbClient.CreateReportAsync(userId, null, null, dto1);
        var result2 = await _reportsDbClient.CreateReportAsync(userId, null, null, dto2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.Id, result2.Id);
        Assert.Equal("First Report", result1.Title);
        Assert.Equal("Second Report", result2.Title);
        Assert.Equal(userId, result1.CreatorUserId);
        Assert.Equal(userId, result2.CreatorUserId);
    }

    [Fact(DisplayName = "Создание репорта с длинным заголовком")]
    public async Task CreateReportAsync_WithLongTitle_ShouldCreateReport()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var longTitle = new string('a', 128); // Максимальная длина согласно валидации
        var dto = new ReportCreateDto
        {
            Title = longTitle
        };

        // Act
        var result = await _reportsDbClient.CreateReportAsync(userId, null, null, dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(longTitle, result.Title);
        Assert.Equal(userId, result.CreatorUserId);
    }

    [Fact(DisplayName = "Проверка что ResponsibleUserId устанавливается равным CreatorUserId")]
    public async Task CreateReportAsync_ShouldSetResponsibleUserIdToCreatorUserId()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var dto = new ReportCreateDto
        {
            Title = "Responsibility Test Report"
        };

        // Act
        var result = await _reportsDbClient.CreateReportAsync(userId, null, null, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(result.CreatorUserId, result.ResponsibleUserId);
        Assert.Equal(userId, result.ResponsibleUserId);
    }

    [Fact(DisplayName = "Проверка что начальный статус репорта - Backlog")]
    public async Task CreateReportAsync_ShouldSetStatusToBacklog()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var dto = new ReportCreateDto
        {
            Title = "Status Test Report"
        };

        // Act
        var result = await _reportsDbClient.CreateReportAsync(userId, null, null, dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Status); // 0 = Backlog
    }
}


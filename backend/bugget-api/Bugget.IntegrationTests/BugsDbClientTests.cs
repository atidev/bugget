using Microsoft.Extensions.DependencyInjection;
using Bugget.DA.Postgres;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.DTO.Report;
using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[Collection("PostgresCollection")]
public class BugsDbClientTests : IClassFixture<AppWithPostgresFixture>
{
    private readonly BugsDbClient _bugsDbClient;
    private readonly ReportsDbClient _reportsDbClient;

    public BugsDbClientTests(AppWithPostgresFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        _bugsDbClient = scope.ServiceProvider.GetRequiredService<BugsDbClient>();
        _reportsDbClient = scope.ServiceProvider.GetRequiredService<ReportsDbClient>();
    }

    #region CreateBugAsync Tests

    [Fact(DisplayName = "Успешное создание бага с минимальными параметрами")]
    public async Task CreateBugAsync_WithMinimalParameters_ShouldCreateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bugDto = new BugDto
        {
            Receive = "Unexpected error",
            Expect = "Success message"
        };

        // Act
        var result = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(bugDto.Receive, result.Receive);
        Assert.Equal(bugDto.Expect, result.Expect);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.True(result.CreatedAt > DateTimeOffset.MinValue);
        Assert.True(result.UpdatedAt > DateTimeOffset.MinValue);
        Assert.Equal(0, result.Status); // Начальный статус
    }

    [Fact(DisplayName = "Успешное создание бага с organizationId")]
    public async Task CreateBugAsync_WithOrganizationId_ShouldCreateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);
        var bugDto = new BugDto
        {
            Receive = "Error 500",
            Expect = "Success response"
        };

        // Act
        var result = await _bugsDbClient.CreateBugAsync(userId, organizationId, report.Id, bugDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(bugDto.Receive, result.Receive);
        Assert.Equal(bugDto.Expect, result.Expect);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.Equal(0, result.Status);
    }

    [Fact(DisplayName = "Успешное создание бага только с Receive")]
    public async Task CreateBugAsync_WithOnlyReceive_ShouldCreateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bugDto = new BugDto
        {
            Receive = "Something went wrong",
            Expect = null
        };

        // Act
        var result = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(bugDto.Receive, result.Receive);
        Assert.Null(result.Expect);
        Assert.Equal(userId, result.CreatorUserId);
    }

    [Fact(DisplayName = "Успешное создание бага только с Expect")]
    public async Task CreateBugAsync_WithOnlyExpect_ShouldCreateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bugDto = new BugDto
        {
            Receive = null,
            Expect = "Expected success"
        };

        // Act
        var result = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Null(result.Receive);
        Assert.Equal(bugDto.Expect, result.Expect);
        Assert.Equal(userId, result.CreatorUserId);
    }

    [Fact(DisplayName = "Создание нескольких багов для одного репорта")]
    public async Task CreateBugAsync_MultipleBugsForOneReport_ShouldCreateSeparateBugs()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bugDto1 = new BugDto { Receive = "First bug", Expect = "First fix" };
        var bugDto2 = new BugDto { Receive = "Second bug", Expect = "Second fix" };

        // Act
        var result1 = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto1);
        var result2 = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.Id, result2.Id);
        Assert.Equal("First bug", result1.Receive);
        Assert.Equal("Second bug", result2.Receive);
        Assert.Equal(userId, result1.CreatorUserId);
        Assert.Equal(userId, result2.CreatorUserId);
    }

    [Fact(DisplayName = "Проверка что начальный статус бага равен 0")]
    public async Task CreateBugAsync_ShouldSetStatusToZero()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bugDto = new BugDto
        {
            Receive = "Test bug",
            Expect = "Test expectation"
        };

        // Act
        var result = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Status);
    }

    [Fact(DisplayName = "Создание бага с длинным текстом Receive")]
    public async Task CreateBugAsync_WithLongReceive_ShouldCreateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var longReceive = new string('a', 2048); // Максимальная длина
        var bugDto = new BugDto
        {
            Receive = longReceive,
            Expect = "Fix needed"
        };

        // Act
        var result = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(longReceive, result.Receive);
    }

    [Fact(DisplayName = "Создание бага с длинным текстом Expect")]
    public async Task CreateBugAsync_WithLongExpect_ShouldCreateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var longExpect = new string('b', 2048); // Максимальная длина
        var bugDto = new BugDto
        {
            Receive = "Bug found",
            Expect = longExpect
        };

        // Act
        var result = await _bugsDbClient.CreateBugAsync(userId, null, report.Id, bugDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(longExpect, result.Expect);
    }

    #endregion

    #region PatchBugAsync Tests

    [Fact(DisplayName = "Успешное обновление Receive бага")]
    public async Task PatchBugAsync_UpdateReceive_ShouldUpdateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var patchDto = new BugPatchDto
        {
            Receive = "Updated receive text"
        };

        // Act
        var result = await _bugsDbClient.PatchBugAsync(report.Id, bug.Id, null, patchDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bug.Id, result.Id);
        Assert.Equal(patchDto.Receive, result.Receive);
        Assert.Equal(bug.Expect, result.Expect); // Expect не изменился
        Assert.True(result.UpdatedAt > bug.UpdatedAt);
    }

    [Fact(DisplayName = "Успешное обновление Expect бага")]
    public async Task PatchBugAsync_UpdateExpect_ShouldUpdateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var patchDto = new BugPatchDto
        {
            Expect = "Updated expect text"
        };

        // Act
        var result = await _bugsDbClient.PatchBugAsync(report.Id, bug.Id, null, patchDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bug.Id, result.Id);
        Assert.Equal(bug.Receive, result.Receive); // Receive не изменился
        Assert.Equal(patchDto.Expect, result.Expect);
        Assert.True(result.UpdatedAt > bug.UpdatedAt);
    }

    [Fact(DisplayName = "Успешное обновление Status бага")]
    public async Task PatchBugAsync_UpdateStatus_ShouldUpdateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var patchDto = new BugPatchDto
        {
            Status = 1 // Изменяем статус на 1
        };

        // Act
        var result = await _bugsDbClient.PatchBugAsync(report.Id, bug.Id, null, patchDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bug.Id, result.Id);
        Assert.Equal(1, result.Status);
        Assert.Equal(bug.Receive, result.Receive); // Receive не изменился
        Assert.Equal(bug.Expect, result.Expect); // Expect не изменился
        Assert.True(result.UpdatedAt > bug.UpdatedAt);
    }

    [Fact(DisplayName = "Успешное обновление всех полей бага")]
    public async Task PatchBugAsync_UpdateAllFields_ShouldUpdateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var patchDto = new BugPatchDto
        {
            Receive = "Completely new receive",
            Expect = "Completely new expect",
            Status = 2
        };

        // Act
        var result = await _bugsDbClient.PatchBugAsync(report.Id, bug.Id, null, patchDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bug.Id, result.Id);
        Assert.Equal(patchDto.Receive, result.Receive);
        Assert.Equal(patchDto.Expect, result.Expect);
        Assert.Equal(patchDto.Status, result.Status);
        Assert.True(result.UpdatedAt > bug.UpdatedAt);
    }

    [Fact(DisplayName = "Обновление бага с organizationId")]
    public async Task PatchBugAsync_WithOrganizationId_ShouldUpdateBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);
        var bug = await CreateTestBugAsync(userId, report.Id, organizationId);
        var patchDto = new BugPatchDto
        {
            Receive = "Updated with org"
        };

        // Act
        var result = await _bugsDbClient.PatchBugAsync(report.Id, bug.Id, organizationId, patchDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bug.Id, result.Id);
        Assert.Equal(patchDto.Receive, result.Receive);
    }

    [Fact(DisplayName = "Обновление бага с пустыми строками")]
    public async Task PatchBugAsync_WithEmptyStrings_ShouldNotUpdateFields()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var patchDto = new BugPatchDto
        {
            Receive = null,
            Expect = null,
            Status = null
        };

        // Act
        var result = await _bugsDbClient.PatchBugAsync(report.Id, bug.Id, null, patchDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bug.Id, result.Id);
        // Проверяем что поля не изменились (значения остались прежними)
        Assert.Equal(bug.Receive, result.Receive);
        Assert.Equal(bug.Expect, result.Expect);
        Assert.Equal(bug.Status, result.Status);
    }

    [Fact(DisplayName = "Обновление UpdatedAt при каждом патче")]
    public async Task PatchBugAsync_ShouldUpdateTimestamp()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var initialUpdatedAt = bug.UpdatedAt;

        // Небольшая задержка для гарантии различия времени
        await Task.Delay(10);

        // Act
        var result = await _bugsDbClient.PatchBugAsync(
            report.Id, 
            bug.Id, 
            null, 
            new BugPatchDto { Receive = "New text" }
        );

        // Assert
        Assert.True(result.UpdatedAt > initialUpdatedAt);
    }

    #endregion

    #region Helper Methods

    private async Task<Bugget.Entities.DbModels.Report.ReportSummaryDbModel> CreateTestReportAsync(
        string userId, 
        string? organizationId = null)
    {
        var reportDto = new ReportV2CreateDto
        {
            Title = $"Test Report {Guid.NewGuid()}"
        };
        return await _reportsDbClient.CreateReportAsync(userId, null, organizationId, reportDto);
    }

    private async Task<Bugget.Entities.DbModels.Bug.BugSummaryDbModel> CreateTestBugAsync(
        string userId, 
        int reportId,
        string? organizationId = null)
    {
        var bugDto = new BugDto
        {
            Receive = "Test bug receive",
            Expect = "Test bug expect"
        };
        return await _bugsDbClient.CreateBugAsync(userId, organizationId, reportId, bugDto);
    }

    #endregion
}


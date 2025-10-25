using Microsoft.Extensions.DependencyInjection;
using Bugget.DA.Postgres;
using Bugget.Entities.DTO.Report;
using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[Collection("PostgresCollection")]
public class ParticipantsDbClientTests : IClassFixture<AppWithPostgresFixture>
{
    private readonly ParticipantsDbClient _participantsDbClient;
    private readonly ReportsDbClient _reportsDbClient;

    public ParticipantsDbClientTests(AppWithPostgresFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        _participantsDbClient = scope.ServiceProvider.GetRequiredService<ParticipantsDbClient>();
        _reportsDbClient = scope.ServiceProvider.GetRequiredService<ReportsDbClient>();
    }

    [Fact(DisplayName = "Добавление первого участника к репорту")]
    public async Task AddParticipantIfNotExistAsync_FirstParticipant_ShouldAddParticipant()
    {
        // Arrange
        var creatorUserId = $"user_{Guid.NewGuid()}";
        var participantUserId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(creatorUserId);

        // Act
        var result = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participantUserId);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(participantUserId, result);
    }

    [Fact(DisplayName = "Добавление того же участника дважды возвращает null")]
    public async Task AddParticipantIfNotExistAsync_SameParticipantTwice_ShouldReturnNull()
    {
        // Arrange
        var creatorUserId = $"user_{Guid.NewGuid()}";
        var participantUserId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(creatorUserId);

        // Act
        var result1 = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participantUserId);
        var result2 = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participantUserId);

        // Assert
        Assert.NotNull(result1); // Первое добавление успешно
        Assert.Contains(participantUserId, result1);
        
        Assert.Null(result2); // Второе добавление возвращает null, так как участник уже существует
    }

    [Fact(DisplayName = "Добавление нескольких разных участников к репорту")]
    public async Task AddParticipantIfNotExistAsync_MultipleParticipants_ShouldAddAll()
    {
        // Arrange
        var creatorUserId = $"user_{Guid.NewGuid()}";
        var participant1 = $"user_{Guid.NewGuid()}";
        var participant2 = $"user_{Guid.NewGuid()}";
        var participant3 = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(creatorUserId);

        // Act
        var result1 = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant1);
        var result2 = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant2);
        var result3 = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant3);

        // Assert
        Assert.NotNull(result3);
        Assert.Contains(participant1, result3);
        Assert.Contains(participant2, result3);
        Assert.Contains(participant3, result3);
        Assert.True(result3.Length >= 3);
    }

    [Fact(DisplayName = "Добавление нового участника после первого")]
    public async Task AddParticipantIfNotExistAsync_SecondParticipant_ShouldAddToList()
    {
        // Arrange
        var creatorUserId = $"user_{Guid.NewGuid()}";
        var participant1 = $"user_{Guid.NewGuid()}";
        var participant2 = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(creatorUserId);

        // Act
        var result1 = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant1);
        var result2 = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Contains(participant1, result1);
        Assert.Contains(participant1, result2); // Первый участник все еще в списке
        Assert.Contains(participant2, result2); // Второй участник добавлен
    }

    [Fact(DisplayName = "Добавление участников к разным репортам")]
    public async Task AddParticipantIfNotExistAsync_DifferentReports_ShouldKeepSeparate()
    {
        // Arrange
        var user1 = $"user_{Guid.NewGuid()}";
        var user2 = $"user_{Guid.NewGuid()}";
        var participant1 = $"user_{Guid.NewGuid()}";
        var participant2 = $"user_{Guid.NewGuid()}";
        var report1 = await CreateTestReportAsync(user1);
        var report2 = await CreateTestReportAsync(user2);

        // Act
        var result1 = await _participantsDbClient.AddParticipantIfNotExistAsync(report1.Id, participant1);
        var result2 = await _participantsDbClient.AddParticipantIfNotExistAsync(report2.Id, participant2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Contains(participant1, result1);
        Assert.DoesNotContain(participant2, result1);
        Assert.Contains(participant2, result2);
        Assert.DoesNotContain(participant1, result2);
    }

    [Fact(DisplayName = "Проверка что список участников не null даже для нового репорта")]
    public async Task AddParticipantIfNotExistAsync_NewReport_ShouldReturnNonNullArray()
    {
        // Arrange
        var creatorUserId = $"user_{Guid.NewGuid()}";
        var participantUserId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(creatorUserId);

        // Act
        var result = await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participantUserId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

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

    #endregion
}


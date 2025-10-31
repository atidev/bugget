using Microsoft.Extensions.DependencyInjection;
using Bugget.DA.Postgres;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.DTO.Report;
using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[Collection("PostgresCollection")]
public class ReportsDbClient_SearchReportsTests : IClassFixture<AppWithPostgresFixture>
{
    private readonly ReportsDbClient _reportsDbClient;
    private readonly BugsDbClient _bugsDbClient;
    private readonly CommentsDbClient _commentsDbClient;
    private readonly ParticipantsDbClient _participantsDbClient;

    public ReportsDbClient_SearchReportsTests(AppWithPostgresFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        _reportsDbClient = scope.ServiceProvider.GetRequiredService<ReportsDbClient>();
        _bugsDbClient = scope.ServiceProvider.GetRequiredService<BugsDbClient>();
        _commentsDbClient = scope.ServiceProvider.GetRequiredService<CommentsDbClient>();
        _participantsDbClient = scope.ServiceProvider.GetRequiredService<ParticipantsDbClient>();
    }

    #region TeamId Filter Tests

    [Fact(DisplayName = "Поиск с фильтром по teamId - возвращает только отчеты данной команды и отчеты без команды")]
    public async Task SearchReportsAsync_FilterByTeamId_ShouldReturnOnlyTeamReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var team1 = $"team_{Guid.NewGuid()}";
        var team2 = $"team_{Guid.NewGuid()}";
        var org = $"org_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(userId, "Report in Team 1", teamId: team1, organizationId: org);
        var report2 = await CreateTestReportAsync(userId, "Another Report in Team 1", teamId: team1, organizationId: org);
        var report3 = await CreateTestReportAsync(userId, "Report in Team 2", teamId: team2, organizationId: org);
        var report4 = await CreateTestReportAsync(userId, "Report without Team", teamId: null, organizationId: org);

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = team1,
            OrganizationId = org,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(3, total);
        Assert.Equal(3, reports.Length);
        Assert.DoesNotContain(reports, r => r.Id == report3.Id);
    }

    [Fact(DisplayName = "Поиск с teamId = null - возвращает отчеты без команды и всех команд")]
    public async Task SearchReportsAsync_TeamIdNull_ShouldReturnAllReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var team1 = $"team_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(userId, "Report with Team", teamId: team1);
        var report2 = await CreateTestReportAsync(userId, "Report without Team", teamId: null);

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.True(total >= 2);
        Assert.Contains(reports, r => r.Id == report1.Id);
        Assert.Contains(reports, r => r.Id == report2.Id);
    }

    [Fact(DisplayName = "Поиск с текстом и фильтром по teamId - применяются оба условия")]
    public async Task SearchReportsAsync_QueryAndTeamId_ShouldApplyBothFilters()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var team1 = $"team_{Guid.NewGuid()}";
        var team2 = $"team_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(userId, "Bug with login", teamId: team1);
        var report2 = await CreateTestReportAsync(userId, "Bug with logout", teamId: team1);
        var report3 = await CreateTestReportAsync(userId, "Bug with login", teamId: team2);

        var searchRequest = new SearchReports
        {
            Query = "login",
            ReportStatuses = null,
            UserIds = null,
            TeamId = team1,
            OrganizationId = null,
            Sort = CreateSortOption("rank", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
        Assert.Equal(team1, reports[0].CreatorTeamId);
    }

    [Fact(DisplayName = "Поиск с teamId и статусами - применяются оба фильтра")]
    public async Task SearchReportsAsync_TeamIdAndStatuses_ShouldApplyBothFilters()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var team1 = $"team_{Guid.NewGuid()}";
        var org = $"org_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(userId, "Report 1", teamId: team1, organizationId: org);
        var report2 = await CreateTestReportAsync(userId, "Report 2", teamId: team1, organizationId: org);
        await _reportsDbClient.PatchReportAsync(report2.Id, null, new ReportPatchDto { Status = 1 });

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = new[] { 0 },
            UserIds = null,
            TeamId = team1,
            OrganizationId = org,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
        Assert.Equal(team1, reports[0].CreatorTeamId);
        Assert.Equal(0, reports[0].Status);
    }

    [Fact(DisplayName = "Поиск с teamId и userIds - применяются оба фильтра")]
    public async Task SearchReportsAsync_TeamIdAndUserIds_ShouldApplyBothFilters()
    {
        // Arrange
        var user1 = $"user_{Guid.NewGuid()}";
        var user2 = $"user_{Guid.NewGuid()}";
        var team1 = $"team_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(user1, "Report by User1 in Team1", teamId: team1);
        await _participantsDbClient.AddParticipantIfNotExistAsync(report1.Id, user2);

        var report2 = await CreateTestReportAsync(user2, "Report by User2 in Team1", teamId: team1);
        var report3 = await CreateTestReportAsync(user1, "Report by User1 without Team", teamId: null);

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = new[] { user2 },
            TeamId = team1,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(2, total);
        Assert.Equal(2, reports.Length);
        Assert.All(reports, r => Assert.Equal(team1, r.CreatorTeamId));
        Assert.Contains(reports, r => r.Id == report1.Id);
        Assert.Contains(reports, r => r.Id == report2.Id);
        Assert.DoesNotContain(reports, r => r.Id == report3.Id);
    }

    [Fact(DisplayName = "Поиск с teamId и organizationId - применяются оба фильтра")]
    public async Task SearchReportsAsync_TeamIdAndOrganizationId_ShouldApplyBothFilters()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var team1 = $"team_{Guid.NewGuid()}";
        var team2 = $"team_{Guid.NewGuid()}";
        var org1 = $"org_{Guid.NewGuid()}";
        var org2 = $"org_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(userId, "Report T1O1", teamId: team1, organizationId: org1);
        var report2 = await CreateTestReportAsync(userId, "Report T1O2", teamId: team1, organizationId: org2);
        var report3 = await CreateTestReportAsync(userId, "Report T2O1", teamId: team2, organizationId: org1);

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = team1,
            OrganizationId = org1,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
        Assert.Equal(team1, reports[0].CreatorTeamId);
    }

    #endregion

    #region General Search Tests

    [Fact(DisplayName = "Поиск без параметров - возвращает все отчеты")]
    public async Task SearchReportsAsync_NoFilters_ShouldReturnAll()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report1 = await CreateTestReportAsync(userId, "Report 1");
        var report2 = await CreateTestReportAsync(userId, "Report 2");

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.True(total >= 2);
        Assert.Contains(reports, r => r.Id == report1.Id);
        Assert.Contains(reports, r => r.Id == report2.Id);
    }

    [Fact(DisplayName = "Поиск по тексту в заголовке отчета")]
    public async Task SearchReportsAsync_QueryInTitle_ShouldReturnMatchingReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report1 = await CreateTestReportAsync(userId, "Critical bug in authentication");
        var report2 = await CreateTestReportAsync(userId, "Bug in payment system");
        var report3 = await CreateTestReportAsync(userId, "Feature request for dashboard");

        var searchRequest = new SearchReports
        {
            Query = "authentication",
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("rank", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
    }

    [Fact(DisplayName = "Поиск по тексту в описании бага")]
    public async Task SearchReportsAsync_QueryInBugDescription_ShouldReturnMatchingReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report1 = await CreateTestReportAsync(userId, "Report 1");
        var report2 = await CreateTestReportAsync(userId, "Report 2");

        await CreateTestBugAsync(userId, report1.Id, "User cannot login with OAuth");
        await CreateTestBugAsync(userId, report2.Id, "Button is not clickable");

        var searchRequest = new SearchReports
        {
            Query = "OAuth",
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("rank", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
    }

    [Fact(DisplayName = "Поиск по тексту в комментариях")]
    public async Task SearchReportsAsync_QueryInComments_ShouldReturnMatchingReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report1 = await CreateTestReportAsync(userId, "Report 1");
        var report2 = await CreateTestReportAsync(userId, "Report 2");

        var bug1 = await CreateTestBugAsync(userId, report1.Id, "Bug 1");
        var bug2 = await CreateTestBugAsync(userId, report2.Id, "Bug 2");

        await CreateTestCommentAsync(userId, report1.Id, bug1.Id, "This is related to database connection");
        await CreateTestCommentAsync(userId, report2.Id, bug2.Id, "UI issue");

        var searchRequest = new SearchReports
        {
            Query = "database",
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("rank", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
    }

    [Fact(DisplayName = "Поиск по статусам")]
    public async Task SearchReportsAsync_FilterByStatus_ShouldReturnMatchingReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report1 = await CreateTestReportAsync(userId, "Report 1");
        var report2 = await CreateTestReportAsync(userId, "Report 2");
        var report3 = await CreateTestReportAsync(userId, "Report 3");

        await _reportsDbClient.PatchReportAsync(report2.Id, null, new ReportPatchDto { Status = 1 });
        await _reportsDbClient.PatchReportAsync(report3.Id, null, new ReportPatchDto { Status = 2 });

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = new[] { 0, 1 },
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.True(total >= 2);
        Assert.Contains(reports, r => r.Id == report1.Id);
        Assert.Contains(reports, r => r.Id == report2.Id);
        Assert.DoesNotContain(reports, r => r.Id == report3.Id);
    }

    [Fact(DisplayName = "Поиск по участникам (userIds)")]
    public async Task SearchReportsAsync_FilterByUserIds_ShouldReturnReportsWithParticipants()
    {
        // Arrange
        var creator = $"user_{Guid.NewGuid()}";
        var participant1 = $"user_{Guid.NewGuid()}";
        var participant2 = $"user_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(creator, "Report 1");
        var report2 = await CreateTestReportAsync(creator, "Report 2");
        var report3 = await CreateTestReportAsync(creator, "Report 3");

        await _participantsDbClient.AddParticipantIfNotExistAsync(report1.Id, participant1);
        await _participantsDbClient.AddParticipantIfNotExistAsync(report2.Id, participant2);

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = new[] { participant1 },
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
    }

    [Fact(DisplayName = "Поиск по organizationId")]
    public async Task SearchReportsAsync_FilterByOrganizationId_ShouldReturnOrgReports()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var org1 = $"org_{Guid.NewGuid()}";
        var org2 = $"org_{Guid.NewGuid()}";

        var report1 = await CreateTestReportAsync(userId, "Report Org1", organizationId: org1);
        var report2 = await CreateTestReportAsync(userId, "Report Org2", organizationId: org2);

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = org1,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.Equal(1, total);
        Assert.Single(reports);
        Assert.Equal(report1.Id, reports[0].Id);
        Assert.DoesNotContain(reports, r => r.Id == report2.Id);
    }

    [Fact(DisplayName = "Сортировка по дате создания (descending)")]
    public async Task SearchReportsAsync_SortByCreatedDesc_ShouldReturnOrderedResults()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report1 = await CreateTestReportAsync(userId, "Report 1");
        await Task.Delay(100); // Ensure different timestamps
        var report2 = await CreateTestReportAsync(userId, "Report 2");
        await Task.Delay(100);
        var report3 = await CreateTestReportAsync(userId, "Report 3");

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        var relevantReports = reports.Where(r => r.Id == report1.Id || r.Id == report2.Id || r.Id == report3.Id).ToArray();
        Assert.True(relevantReports.Length >= 3);
        Assert.True(relevantReports[0].CreatedAt >= relevantReports[1].CreatedAt);
        Assert.True(relevantReports[1].CreatedAt >= relevantReports[2].CreatedAt);
    }

    [Fact(DisplayName = "Пагинация работает корректно")]
    public async Task SearchReportsAsync_Pagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        for (int i = 0; i < 5; i++)
        {
            await CreateTestReportAsync(userId, $"Report {i}");
        }

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 2,
            Take = 2
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        Assert.True(total >= 5);
        Assert.Equal(2, reports.Length);
    }

    [Fact(DisplayName = "Полный граф данных возвращается корректно")]
    public async Task SearchReportsAsync_CompleteGraph_ShouldReturnFullData()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, "Full Graph Report");

        var participant = $"user_{Guid.NewGuid()}";
        await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant);

        var bug = await CreateTestBugAsync(userId, report.Id, "Bug with comment");
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test comment");

        var searchRequest = new SearchReports
        {
            Query = null,
            ReportStatuses = null,
            UserIds = null,
            TeamId = null,
            OrganizationId = null,
            Sort = CreateSortOption("created", true),
            Skip = 0,
            Take = 10
        };

        // Act
        var (total, reports) = await _reportsDbClient.SearchReportsAsync(searchRequest);

        // Assert
        var result = reports.FirstOrDefault(r => r.Id == report.Id);
        Assert.NotNull(result);
        Assert.Contains(participant, result.ParticipantsUserIds);
        Assert.Single(result.Bugs);
        Assert.Equal(bug.Id, result.Bugs[0].Id);
        Assert.Single(result.Bugs[0].Comments);
        Assert.Equal(comment.Id, result.Bugs[0].Comments[0].Id);
    }

    #endregion

    #region Helper Methods

    private async Task<Bugget.Entities.DbModels.Report.ReportSummaryDbModel> CreateTestReportAsync(
        string userId,
        string title,
        string? teamId = null,
        string? organizationId = null)
    {
        var reportDto = new ReportCreateDto
        {
            Title = title
        };
        return await _reportsDbClient.CreateReportAsync(userId, teamId, organizationId, reportDto);
    }

    private async Task<Bugget.Entities.DbModels.Bug.BugSummaryDbModel> CreateTestBugAsync(
        string userId,
        int reportId,
        string receive,
        string? organizationId = null)
    {
        var bugDto = new BugDto
        {
            Receive = receive,
            Expect = "Expected behavior"
        };
        return await _bugsDbClient.CreateBugAsync(userId, organizationId, reportId, bugDto);
    }

    private async Task<Bugget.Entities.DbModels.Comment.CommentSummaryDbModel> CreateTestCommentAsync(
        string userId,
        int reportId,
        int bugId,
        string text,
        string? organizationId = null)
    {
        return await _commentsDbClient.CreateCommentAsync(organizationId, userId, reportId, bugId, text);
    }

    private static SortOption CreateSortOption(string field, bool isDescending)
    {
        // Use reflection to create SortOption since it has private init
        var sortOption = Activator.CreateInstance(typeof(SortOption), true);
        typeof(SortOption).GetProperty("Field")!.SetValue(sortOption, field);
        typeof(SortOption).GetProperty("IsDescending")!.SetValue(sortOption, isDescending);
        return (SortOption)sortOption!;
    }

    #endregion
}


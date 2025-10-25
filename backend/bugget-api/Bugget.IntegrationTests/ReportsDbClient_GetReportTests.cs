using Microsoft.Extensions.DependencyInjection;
using Bugget.DA.Postgres;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.DTO.Report;
using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[Collection("PostgresCollection")]
public class ReportsDbClient_GetReportTests : IClassFixture<AppWithPostgresFixture>
{
    private readonly ReportsDbClient _reportsDbClient;
    private readonly BugsDbClient _bugsDbClient;
    private readonly CommentsDbClient _commentsDbClient;
    private readonly ParticipantsDbClient _participantsDbClient;
    private readonly AttachmentDbClient _attachmentDbClient;

    // AttachType константы
    private const int AttachType_BugFact = 0;
    private const int AttachType_BugExpected = 1;
    private const int AttachType_Comment = 2;

    public ReportsDbClient_GetReportTests(AppWithPostgresFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        _reportsDbClient = scope.ServiceProvider.GetRequiredService<ReportsDbClient>();
        _bugsDbClient = scope.ServiceProvider.GetRequiredService<BugsDbClient>();
        _commentsDbClient = scope.ServiceProvider.GetRequiredService<CommentsDbClient>();
        _participantsDbClient = scope.ServiceProvider.GetRequiredService<ParticipantsDbClient>();
        _attachmentDbClient = scope.ServiceProvider.GetRequiredService<AttachmentDbClient>();
    }

    [Fact(DisplayName = "Получение простого репорта без багов")]
    public async Task GetReportAsync_EmptyReport_ShouldReturnReport()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var created = await CreateTestReportAsync(userId);

        // Act
        var result = await _reportsDbClient.GetReportAsync(created.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(created.Title, result.Title);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.Equal(userId, result.ResponsibleUserId);
        Assert.NotNull(result.Bugs);
        Assert.Empty(result.Bugs);
        Assert.NotNull(result.ParticipantsUserIds);
    }

    [Fact(DisplayName = "Получение репорта с одним багом")]
    public async Task GetReportAsync_WithOneBug_ShouldReturnReportWithBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(report.Id, result.Id);
        Assert.NotNull(result.Bugs);
        Assert.Single(result.Bugs);
        Assert.Equal(bug.Id, result.Bugs[0].Id);
        Assert.Equal(bug.Receive, result.Bugs[0].Receive);
        Assert.Equal(bug.Expect, result.Bugs[0].Expect);
        Assert.NotNull(result.Bugs[0].Comments);
        Assert.Empty(result.Bugs[0].Comments);
    }

    [Fact(DisplayName = "Получение репорта с несколькими багами")]
    public async Task GetReportAsync_WithMultipleBugs_ShouldReturnAllBugs()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug1 = await CreateTestBugAsync(userId, report.Id, "Bug 1 receive", "Bug 1 expect");
        var bug2 = await CreateTestBugAsync(userId, report.Id, "Bug 2 receive", "Bug 2 expect");
        var bug3 = await CreateTestBugAsync(userId, report.Id, "Bug 3 receive", "Bug 3 expect");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Bugs.Length);
        Assert.Contains(result.Bugs, b => b.Id == bug1.Id);
        Assert.Contains(result.Bugs, b => b.Id == bug2.Id);
        Assert.Contains(result.Bugs, b => b.Id == bug3.Id);
    }

    [Fact(DisplayName = "Получение репорта с багом и комментариями")]
    public async Task GetReportAsync_WithBugAndComments_ShouldReturnCommentsInBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment1 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Comment 1");
        var comment2 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Comment 2");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Bugs);
        var bugResult = result.Bugs[0];
        Assert.Equal(2, bugResult.Comments.Length);
        Assert.Contains(bugResult.Comments, c => c.Id == comment1.Id && c.Text == "Comment 1");
        Assert.Contains(bugResult.Comments, c => c.Id == comment2.Id && c.Text == "Comment 2");
    }

    [Fact(DisplayName = "Получение репорта с багом и вложениями бага")]
    public async Task GetReportAsync_WithBugAttachments_ShouldReturnAttachmentsInBug()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var attachment1 = await CreateTestBugAttachmentAsync(userId, bug.Id, "file1.jpg");
        var attachment2 = await CreateTestBugAttachmentAsync(userId, bug.Id, "file2.png");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Bugs);
        var bugResult = result.Bugs[0];
        Assert.Equal(2, bugResult.Attachments.Length);
        Assert.Contains(bugResult.Attachments, a => a.Id == attachment1.Id && a.FileName == "file1.jpg");
        Assert.Contains(bugResult.Attachments, a => a.Id == attachment2.Id && a.FileName == "file2.png");
    }

    [Fact(DisplayName = "Получение репорта с комментарием и вложениями комментария")]
    public async Task GetReportAsync_WithCommentAttachments_ShouldReturnAttachmentsInComment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test comment");
        var attachment = await CreateTestCommentAttachmentAsync(userId, comment.Id, "comment_file.pdf");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Bugs);
        var bugResult = result.Bugs[0];
        Assert.Single(bugResult.Comments);
        var commentResult = bugResult.Comments[0];
        Assert.Single(commentResult.Attachments);
        Assert.Equal(attachment.Id, commentResult.Attachments[0].Id);
        Assert.Equal("comment_file.pdf", commentResult.Attachments[0].FileName);
    }

    [Fact(DisplayName = "Получение репорта с участниками")]
    public async Task GetReportAsync_WithParticipants_ShouldReturnParticipants()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var participant1 = $"user_{Guid.NewGuid()}";
        var participant2 = $"user_{Guid.NewGuid()}";
        
        await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant1);
        await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant2);

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.ParticipantsUserIds);
        Assert.Contains(participant1, result.ParticipantsUserIds);
        Assert.Contains(participant2, result.ParticipantsUserIds);
    }

    [Fact(DisplayName = "Получение полного репорта со всеми связанными данными")]
    public async Task GetReportAsync_CompleteReport_ShouldReturnFullGraph()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        
        // Добавляем участников
        var participant1 = $"user_{Guid.NewGuid()}";
        await _participantsDbClient.AddParticipantIfNotExistAsync(report.Id, participant1);
        
        // Создаем баг с комментариями и вложениями
        var bug = await CreateTestBugAsync(userId, report.Id);
        var bugAttachment = await CreateTestBugAttachmentAsync(userId, bug.Id, "bug_file.jpg");
        
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test comment");
        var commentAttachment = await CreateTestCommentAttachmentAsync(userId, comment.Id, "comment_file.pdf");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(report.Id, result.Id);
        
        // Проверяем участников
        Assert.Contains(participant1, result.ParticipantsUserIds);
        
        // Проверяем баги
        Assert.Single(result.Bugs);
        var bugResult = result.Bugs[0];
        Assert.Equal(bug.Id, bugResult.Id);
        
        // Проверяем вложения бага
        Assert.Single(bugResult.Attachments);
        Assert.Equal(bugAttachment.Id, bugResult.Attachments[0].Id);
        
        // Проверяем комментарии
        Assert.Single(bugResult.Comments);
        var commentResult = bugResult.Comments[0];
        Assert.Equal(comment.Id, commentResult.Id);
        
        // Проверяем вложения комментария
        Assert.Single(commentResult.Attachments);
        Assert.Equal(commentAttachment.Id, commentResult.Attachments[0].Id);
    }

    [Fact(DisplayName = "Получение несуществующего репорта возвращает null")]
    public async Task GetReportAsync_NonExistentReport_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = 999999;

        // Act
        var result = await _reportsDbClient.GetReportAsync(nonExistentId, null);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Получение репорта с organizationId")]
    public async Task GetReportAsync_WithOrganizationId_ShouldReturnReport()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);
        var bug = await CreateTestBugAsync(userId, report.Id, organizationId: organizationId);

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, organizationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(report.Id, result.Id);
        Assert.Single(result.Bugs);
    }

    [Fact(DisplayName = "Получение репорта с неправильным organizationId возвращает null")]
    public async Task GetReportAsync_WithWrongOrganizationId_ShouldReturnNull()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var wrongOrganizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, wrongOrganizationId);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Комментарии группируются по багам корректно")]
    public async Task GetReportAsync_MultipleБugsWithComments_ShouldGroupCommentsCorrectly()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        
        var bug1 = await CreateTestBugAsync(userId, report.Id, "Bug 1", "Fix 1");
        var bug2 = await CreateTestBugAsync(userId, report.Id, "Bug 2", "Fix 2");
        
        var comment1ForBug1 = await CreateTestCommentAsync(userId, report.Id, bug1.Id, "Bug1 Comment1");
        var comment2ForBug1 = await CreateTestCommentAsync(userId, report.Id, bug1.Id, "Bug1 Comment2");
        var comment1ForBug2 = await CreateTestCommentAsync(userId, report.Id, bug2.Id, "Bug2 Comment1");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Bugs.Length);
        
        var bug1Result = result.Bugs.First(b => b.Id == bug1.Id);
        var bug2Result = result.Bugs.First(b => b.Id == bug2.Id);
        
        Assert.Equal(2, bug1Result.Comments.Length);
        Assert.Single(bug2Result.Comments);
        
        Assert.Contains(bug1Result.Comments, c => c.Text == "Bug1 Comment1");
        Assert.Contains(bug1Result.Comments, c => c.Text == "Bug1 Comment2");
        Assert.Equal("Bug2 Comment1", bug2Result.Comments[0].Text);
    }

    [Fact(DisplayName = "Вложения группируются по типам корректно")]
    public async Task GetReportAsync_WithDifferentAttachmentTypes_ShouldGroupCorrectly()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");
        
        // Вложения к багу (AttachType 0 и 1)
        var bugAttachment = await CreateTestBugAttachmentAsync(userId, bug.Id, "bug.jpg");
        
        // Вложение к комментарию (AttachType 2)
        var commentAttachment = await CreateTestCommentAttachmentAsync(userId, comment.Id, "comment.pdf");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        var bugResult = result.Bugs[0];
        var commentResult = bugResult.Comments[0];
        
        // Вложения бага не должны содержать вложения комментария
        Assert.Single(bugResult.Attachments);
        Assert.Equal(bugAttachment.Id, bugResult.Attachments[0].Id);
        Assert.All(bugResult.Attachments, a => Assert.NotEqual(AttachType_Comment, a.AttachType));
        
        // Вложения комментария не должны содержать вложения бага
        Assert.Single(commentResult.Attachments);
        Assert.Equal(commentAttachment.Id, commentResult.Attachments[0].Id);
        Assert.All(commentResult.Attachments, a => Assert.Equal(AttachType_Comment, a.AttachType));
    }

    [Fact(DisplayName = "Комментарии сортируются по времени создания")]
    public async Task GetReportAsync_Comments_ShouldBeSortedByCreatedAt()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        
        // Создаем комментарии с небольшими задержками
        var comment1 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "First");
        await Task.Delay(10);
        var comment2 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Second");
        await Task.Delay(10);
        var comment3 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Third");

        // Act
        var result = await _reportsDbClient.GetReportAsync(report.Id, null);

        // Assert
        Assert.NotNull(result);
        var comments = result.Bugs[0].Comments;
        Assert.Equal(3, comments.Length);
        
        // Проверяем что комментарии отсортированы по CreatedAt
        Assert.True(comments[0].CreatedAt <= comments[1].CreatedAt);
        Assert.True(comments[1].CreatedAt <= comments[2].CreatedAt);
        
        Assert.Equal("First", comments[0].Text);
        Assert.Equal("Second", comments[1].Text);
        Assert.Equal("Third", comments[2].Text);
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

    private async Task<Bugget.Entities.DbModels.Bug.BugSummaryDbModel> CreateTestBugAsync(
        string userId,
        int reportId,
        string? receive = null,
        string? expect = null,
        string? organizationId = null)
    {
        var bugDto = new BugDto
        {
            Receive = receive ?? "Test bug receive",
            Expect = expect ?? "Test bug expect"
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

    private async Task<AttachmentDbModel> CreateTestBugAttachmentAsync(
        string userId,
        int bugId,
        string fileName)
    {
        var createModel = new CreateAttachmentDbModel
        {
            EntityId = bugId,
            AttachType = AttachType_BugFact,
            StorageKey = $"test/bug_{bugId}_{Guid.NewGuid()}.jpg",
            StorageKind = 1,
            CreatorUserId = userId,
            LengthBytes = 1024,
            FileName = fileName,
            MimeType = "image/jpeg"
        };
        return await _attachmentDbClient.CreateAttachment(createModel);
    }

    private async Task<AttachmentDbModel> CreateTestCommentAttachmentAsync(
        string userId,
        int commentId,
        string fileName)
    {
        var createModel = new CreateAttachmentDbModel
        {
            EntityId = commentId,
            AttachType = AttachType_Comment,
            StorageKey = $"test/comment_{commentId}_{Guid.NewGuid()}.pdf",
            StorageKind = 1,
            CreatorUserId = userId,
            LengthBytes = 2048,
            FileName = fileName,
            MimeType = "application/pdf"
        };
        return await _attachmentDbClient.CreateAttachment(createModel);
    }

    #endregion
}


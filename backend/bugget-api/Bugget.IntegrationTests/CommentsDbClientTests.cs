using Microsoft.Extensions.DependencyInjection;
using Bugget.DA.Postgres;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.DTO.Report;
using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[Collection("PostgresCollection")]
public class CommentsDbClientTests : IClassFixture<AppWithPostgresFixture>
{
    private readonly CommentsDbClient _commentsDbClient;
    private readonly BugsDbClient _bugsDbClient;
    private readonly ReportsDbClient _reportsDbClient;

    public CommentsDbClientTests(AppWithPostgresFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        _commentsDbClient = scope.ServiceProvider.GetRequiredService<CommentsDbClient>();
        _bugsDbClient = scope.ServiceProvider.GetRequiredService<BugsDbClient>();
        _reportsDbClient = scope.ServiceProvider.GetRequiredService<ReportsDbClient>();
    }

    #region CreateCommentAsync Tests

    [Fact(DisplayName = "Успешное создание комментария с минимальными параметрами")]
    public async Task CreateCommentAsync_WithMinimalParameters_ShouldCreateComment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var commentText = "This is a test comment";

        // Act
        var result = await _commentsDbClient.CreateCommentAsync(null, userId, report.Id, bug.Id, commentText);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(bug.Id, result.BugId);
        Assert.Equal(commentText, result.Text);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.True(result.CreatedAt > DateTimeOffset.MinValue);
        Assert.True(result.UpdatedAt > DateTimeOffset.MinValue);
        Assert.Equal(result.CreatedAt, result.UpdatedAt); // При создании времена должны совпадать
    }

    [Fact(DisplayName = "Успешное создание комментария с organizationId")]
    public async Task CreateCommentAsync_WithOrganizationId_ShouldCreateComment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);
        var bug = await CreateTestBugAsync(userId, report.Id, organizationId);
        var commentText = "Comment with organization";

        // Act
        var result = await _commentsDbClient.CreateCommentAsync(organizationId, userId, report.Id, bug.Id, commentText);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(bug.Id, result.BugId);
        Assert.Equal(commentText, result.Text);
        Assert.Equal(userId, result.CreatorUserId);
    }

    [Fact(DisplayName = "Создание нескольких комментариев к одному багу")]
    public async Task CreateCommentAsync_MultipleCommentsForOneBug_ShouldCreateSeparateComments()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment1Text = "First comment";
        var comment2Text = "Second comment";
        var comment3Text = "Third comment";

        // Act
        var result1 = await _commentsDbClient.CreateCommentAsync(null, userId, report.Id, bug.Id, comment1Text);
        var result2 = await _commentsDbClient.CreateCommentAsync(null, userId, report.Id, bug.Id, comment2Text);
        var result3 = await _commentsDbClient.CreateCommentAsync(null, userId, report.Id, bug.Id, comment3Text);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
        Assert.NotEqual(result1.Id, result2.Id);
        Assert.NotEqual(result2.Id, result3.Id);
        Assert.NotEqual(result1.Id, result3.Id);
        Assert.Equal(comment1Text, result1.Text);
        Assert.Equal(comment2Text, result2.Text);
        Assert.Equal(comment3Text, result3.Text);
        Assert.Equal(bug.Id, result1.BugId);
        Assert.Equal(bug.Id, result2.BugId);
        Assert.Equal(bug.Id, result3.BugId);
    }

    [Fact(DisplayName = "Создание комментариев разными пользователями")]
    public async Task CreateCommentAsync_DifferentUsers_ShouldCreateComments()
    {
        // Arrange
        var user1 = $"user_{Guid.NewGuid()}";
        var user2 = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(user1);
        var bug = await CreateTestBugAsync(user1, report.Id);

        // Act
        var comment1 = await _commentsDbClient.CreateCommentAsync(null, user1, report.Id, bug.Id, "Comment by user1");
        var comment2 = await _commentsDbClient.CreateCommentAsync(null, user2, report.Id, bug.Id, "Comment by user2");

        // Assert
        Assert.NotNull(comment1);
        Assert.NotNull(comment2);
        Assert.NotEqual(comment1.Id, comment2.Id);
        Assert.Equal(user1, comment1.CreatorUserId);
        Assert.Equal(user2, comment2.CreatorUserId);
        Assert.Equal(bug.Id, comment1.BugId);
        Assert.Equal(bug.Id, comment2.BugId);
    }

    #endregion

    #region UpdateCommentAsync Tests

    [Fact(DisplayName = "Успешное обновление текста комментария")]
    public async Task UpdateCommentAsync_WithNewText_ShouldUpdateComment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Original text");
        var newText = "Updated text";

        // Act
        var result = await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment.Id, newText);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
        Assert.Equal(newText, result.Text);
        Assert.Equal(bug.Id, result.BugId);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.True(result.UpdatedAt > comment.UpdatedAt);
        Assert.Equal(comment.CreatedAt, result.CreatedAt); // CreatedAt не должен измениться
    }

    [Fact(DisplayName = "Обновление комментария с organizationId")]
    public async Task UpdateCommentAsync_WithOrganizationId_ShouldUpdateComment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);
        var bug = await CreateTestBugAsync(userId, report.Id, organizationId);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Original", organizationId);
        var newText = "Updated with org";

        // Act
        var result = await _commentsDbClient.UpdateCommentAsync(organizationId, userId, report.Id, bug.Id, comment.Id, newText);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(comment.Id, result.Id);
        Assert.Equal(newText, result.Text);
    }

    [Fact(DisplayName = "Многократное обновление комментария")]
    public async Task UpdateCommentAsync_MultipleUpdates_ShouldUpdateCorrectly()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Original text");

        // Act & Assert - First update
        var result1 = await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment.Id, "First update");
        Assert.NotNull(result1);
        Assert.Equal("First update", result1.Text);
        Assert.True(result1.UpdatedAt > comment.UpdatedAt);

        // Небольшая задержка
        await Task.Delay(10);

        // Act & Assert - Second update
        var result2 = await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment.Id, "Second update");
        Assert.NotNull(result2);
        Assert.Equal("Second update", result2.Text);
        Assert.True(result2.UpdatedAt > result1.UpdatedAt);

        // CreatedAt должен остаться неизменным
        Assert.Equal(comment.CreatedAt, result1.CreatedAt);
        Assert.Equal(comment.CreatedAt, result2.CreatedAt);
    }

    [Fact(DisplayName = "Проверка что UpdatedAt обновляется при каждом изменении")]
    public async Task UpdateCommentAsync_ShouldUpdateTimestamp()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Original");
        var initialUpdatedAt = comment.UpdatedAt;

        // Небольшая задержка для гарантии различия времени
        await Task.Delay(10);

        // Act
        var result = await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment.Id, "Updated");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.UpdatedAt > initialUpdatedAt);
    }

    #endregion

    #region DeleteCommentAsync Tests

    [Fact(DisplayName = "Успешное удаление комментария")]
    public async Task DeleteCommentAsync_WithValidComment_ShouldDeleteComment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "To be deleted");

        // Act
        await _commentsDbClient.DeleteCommentAsync(null, userId, report.Id, bug.Id, comment.Id);

        // Assert
        // Проверяем что комментарий удален, попытка обновить должна выбросить исключение
        await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
            await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment.Id, "Should not update"));
    }

    [Fact(DisplayName = "Удаление комментария с organizationId")]
    public async Task DeleteCommentAsync_WithOrganizationId_ShouldDeleteComment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);
        var bug = await CreateTestBugAsync(userId, report.Id, organizationId);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "To be deleted", organizationId);

        // Act
        await _commentsDbClient.DeleteCommentAsync(organizationId, userId, report.Id, bug.Id, comment.Id);

        // Assert
        await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
            await _commentsDbClient.UpdateCommentAsync(organizationId, userId, report.Id, bug.Id, comment.Id, "Should not update"));
    }

    [Fact(DisplayName = "Удаление одного из нескольких комментариев")]
    public async Task DeleteCommentAsync_OneOfMultiple_ShouldDeleteOnlyOne()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment1 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Comment 1");
        var comment2 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Comment 2");
        var comment3 = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Comment 3");

        // Act - удаляем второй комментарий
        await _commentsDbClient.DeleteCommentAsync(null, userId, report.Id, bug.Id, comment2.Id);

        // Assert - проверяем что comment2 удален (выбрасывает исключение)
        await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
            await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment2.Id, "Should not update"));

        // Assert - проверяем что comment1 и comment3 все еще существуют
        var updateResult1 = await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment1.Id, "Updated 1");
        var updateResult3 = await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment3.Id, "Updated 3");
        Assert.NotNull(updateResult1);
        Assert.NotNull(updateResult3);
        Assert.Equal("Updated 1", updateResult1.Text);
        Assert.Equal("Updated 3", updateResult3.Text);
    }

    [Fact(DisplayName = "Удаление несуществующего комментария должно выбросить исключение")]
    public async Task DeleteCommentAsync_NonExistentComment_ShouldThrow()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var nonExistentCommentId = 999999;

        // Act & Assert - должно быть исключение
        await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
            await _commentsDbClient.DeleteCommentAsync(null, userId, report.Id, bug.Id, nonExistentCommentId));
    }

    #endregion

    #region Complex Workflow Tests

    [Fact(DisplayName = "Полный жизненный цикл комментария: создание -> обновление -> удаление")]
    public async Task CommentLifecycle_CreateUpdateDelete_ShouldWorkCorrectly()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);

        // Act & Assert - Создание
        var comment = await _commentsDbClient.CreateCommentAsync(null, userId, report.Id, bug.Id, "Initial text");
        Assert.NotNull(comment);
        Assert.Equal("Initial text", comment.Text);

        // Act & Assert - Обновление
        var updated = await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment.Id, "Updated text");
        Assert.NotNull(updated);
        Assert.Equal("Updated text", updated.Text);
        Assert.Equal(comment.Id, updated.Id);

        // Act & Assert - Удаление
        await _commentsDbClient.DeleteCommentAsync(null, userId, report.Id, bug.Id, comment.Id);
        // После удаления попытка обновления должна выбросить исключение
        await Assert.ThrowsAsync<Npgsql.PostgresException>(async () =>
            await _commentsDbClient.UpdateCommentAsync(null, userId, report.Id, bug.Id, comment.Id, "Should fail"));
    }

    [Fact(DisplayName = "Создание комментариев к разным багам одного репорта")]
    public async Task CreateCommentAsync_DifferentBugsInSameReport_ShouldCreateCorrectly()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug1 = await CreateTestBugAsync(userId, report.Id);
        var bug2 = await CreateTestBugAsync(userId, report.Id);

        // Act
        var comment1 = await _commentsDbClient.CreateCommentAsync(null, userId, report.Id, bug1.Id, "Comment for bug 1");
        var comment2 = await _commentsDbClient.CreateCommentAsync(null, userId, report.Id, bug2.Id, "Comment for bug 2");

        // Assert
        Assert.NotNull(comment1);
        Assert.NotNull(comment2);
        Assert.NotEqual(comment1.Id, comment2.Id);
        Assert.Equal(bug1.Id, comment1.BugId);
        Assert.Equal(bug2.Id, comment2.BugId);
        Assert.Equal("Comment for bug 1", comment1.Text);
        Assert.Equal("Comment for bug 2", comment2.Text);
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

    private async Task<Bugget.Entities.DbModels.Comment.CommentSummaryDbModel> CreateTestCommentAsync(
        string userId,
        int reportId,
        int bugId,
        string text,
        string? organizationId = null)
    {
        return await _commentsDbClient.CreateCommentAsync(organizationId, userId, reportId, bugId, text);
    }

    #endregion
}


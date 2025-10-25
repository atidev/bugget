using Microsoft.Extensions.DependencyInjection;
using Bugget.DA.Postgres;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.DTO.Report;
using Bugget.IntegrationTests.Fixtures;
using Xunit;

namespace Bugget.IntegrationTests;

[Collection("PostgresCollection")]
public class AttachmentDbClientTests : IClassFixture<AppWithPostgresFixture>
{
    private readonly AttachmentDbClient _attachmentDbClient;
    private readonly BugsDbClient _bugsDbClient;
    private readonly CommentsDbClient _commentsDbClient;
    private readonly ReportsDbClient _reportsDbClient;

    // AttachType константы (из документации/схемы БД)
    private const int AttachType_BugFact = 0;      // Вложение к receive бага
    private const int AttachType_BugExpected = 1;  // Вложение к expect бага
    private const int AttachType_Comment = 2;      // Вложение к комментарию

    public AttachmentDbClientTests(AppWithPostgresFixture fixture)
    {
        using var scope = fixture.Services.CreateScope();
        _attachmentDbClient = scope.ServiceProvider.GetRequiredService<AttachmentDbClient>();
        _bugsDbClient = scope.ServiceProvider.GetRequiredService<BugsDbClient>();
        _commentsDbClient = scope.ServiceProvider.GetRequiredService<CommentsDbClient>();
        _reportsDbClient = scope.ServiceProvider.GetRequiredService<ReportsDbClient>();
    }

    #region CreateAttachment Tests

    [Fact(DisplayName = "Успешное создание вложения для бага")]
    public async Task CreateAttachment_ForBug_ShouldCreateAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var createModel = new CreateAttachmentDbModel
        {
            EntityId = bug.Id,
            AttachType = AttachType_BugFact,
            StorageKey = $"attachments/bug_{bug.Id}_{Guid.NewGuid()}.jpg",
            StorageKind = 1, // Standard
            CreatorUserId = userId,
            LengthBytes = 1024000,
            FileName = "test_image.jpg",
            MimeType = "image/jpeg"
        };

        // Act
        var result = await _attachmentDbClient.CreateAttachment(createModel);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(bug.Id, result.EntityId);
        Assert.Equal(AttachType_BugFact, result.AttachType);
        Assert.Equal(createModel.StorageKey, result.StorageKey);
        Assert.Equal(createModel.StorageKind, result.StorageKind);
        Assert.Equal(userId, result.CreatorUserId);
        Assert.Equal(createModel.LengthBytes, result.LengthBytes);
        Assert.Equal(createModel.FileName, result.FileName);
        Assert.Equal(createModel.MimeType, result.MimeType);
        Assert.True(result.CreatedAt > DateTimeOffset.MinValue);
    }

    [Fact(DisplayName = "Успешное создание вложения для комментария")]
    public async Task CreateAttachment_ForComment_ShouldCreateAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test comment");
        var createModel = new CreateAttachmentDbModel
        {
            EntityId = comment.Id,
            AttachType = AttachType_Comment,
            StorageKey = $"attachments/comment_{comment.Id}_{Guid.NewGuid()}.pdf",
            StorageKind = 1,
            CreatorUserId = userId,
            LengthBytes = 512000,
            FileName = "document.pdf",
            MimeType = "application/pdf"
        };

        // Act
        var result = await _attachmentDbClient.CreateAttachment(createModel);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(comment.Id, result.EntityId);
        Assert.Equal(AttachType_Comment, result.AttachType);
        Assert.Equal(createModel.FileName, result.FileName);
        Assert.Equal(createModel.MimeType, result.MimeType);
    }

    [Fact(DisplayName = "Создание нескольких вложений для одного бага")]
    public async Task CreateAttachment_MultipleBugAttachments_ShouldCreateAll()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);

        var attachment1 = new CreateAttachmentDbModel
        {
            EntityId = bug.Id,
            AttachType = AttachType_BugFact,
            StorageKey = $"attachments/bug_{bug.Id}_1.jpg",
            StorageKind = 1,
            CreatorUserId = userId,
            LengthBytes = 1000,
            FileName = "file1.jpg",
            MimeType = "image/jpeg"
        };

        var attachment2 = new CreateAttachmentDbModel
        {
            EntityId = bug.Id,
            AttachType = AttachType_BugFact,
            StorageKey = $"attachments/bug_{bug.Id}_2.png",
            StorageKind = 1,
            CreatorUserId = userId,
            LengthBytes = 2000,
            FileName = "file2.png",
            MimeType = "image/png"
        };

        // Act
        var result1 = await _attachmentDbClient.CreateAttachment(attachment1);
        var result2 = await _attachmentDbClient.CreateAttachment(attachment2);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotEqual(result1.Id, result2.Id);
        Assert.Equal(bug.Id, result1.EntityId);
        Assert.Equal(bug.Id, result2.EntityId);
    }

    #endregion

    #region UpdateAttachment Tests

    [Fact(DisplayName = "Успешное обновление вложения")]
    public async Task UpdateAttachmentAsync_WithNewData_ShouldUpdateAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var attachment = await CreateTestBugAttachmentAsync(userId, bug.Id);

        var updateModel = new UpdateAttachmentDbModel
        {
            Id = attachment.Id,
            StorageKey = "updated/storage/key.jpg",
            StorageKind = 2, // Cold storage
            LengthBytes = 2048000,
            FileName = "updated_filename.jpg",
            MimeType = "image/jpeg",
            HasPreview = true,
            IsGzipCompressed = true
        };

        // Act
        var result = await _attachmentDbClient.UpdateAttachmentAsync(updateModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachment.Id, result.Id);
        Assert.Equal(updateModel.StorageKey, result.StorageKey);
        Assert.Equal(updateModel.StorageKind, result.StorageKind);
        Assert.Equal(updateModel.LengthBytes, result.LengthBytes);
        Assert.Equal(updateModel.FileName, result.FileName);
        Assert.Equal(updateModel.MimeType, result.MimeType);
        Assert.Equal(updateModel.HasPreview, result.HasPreview);
        Assert.Equal(updateModel.IsGzipCompressed, result.IsGzipCompressed);
    }

    [Fact(DisplayName = "Обновление только имени файла")]
    public async Task UpdateAttachmentAsync_OnlyFileName_ShouldUpdate()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var attachment = await CreateTestBugAttachmentAsync(userId, bug.Id);

        var updateModel = new UpdateAttachmentDbModel
        {
            Id = attachment.Id,
            StorageKey = attachment.StorageKey!,
            StorageKind = attachment.StorageKind ?? 1,
            LengthBytes = attachment.LengthBytes ?? 0,
            FileName = "completely_new_name.jpg",
            MimeType = attachment.MimeType,
            HasPreview = attachment.HasPreview ?? false,
            IsGzipCompressed = attachment.IsGzipCompressed ?? false
        };

        // Act
        var result = await _attachmentDbClient.UpdateAttachmentAsync(updateModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("completely_new_name.jpg", result.FileName);
        Assert.Equal(attachment.StorageKey, result.StorageKey);
    }

    #endregion

    #region GetBugAttachment Tests

    [Fact(DisplayName = "Получение вложения бага по ID")]
    public async Task GetBugAttachmentAsync_ValidAttachment_ShouldReturnAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var attachment = await CreateTestBugAttachmentAsync(userId, bug.Id);

        // Act
        var result = await _attachmentDbClient.GetBugAttachmentAsync(null, report.Id, bug.Id, attachment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachment.Id, result.Id);
        Assert.Equal(attachment.FileName, result.FileName);
    }

    [Fact(DisplayName = "Получение несуществующего вложения бага")]
    public async Task GetBugAttachmentAsync_NonExistent_ShouldReturnNull()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);

        // Act
        var result = await _attachmentDbClient.GetBugAttachmentAsync(null, report.Id, bug.Id, 999999);

        // Assert
        Assert.Null(result);
    }

    [Fact(DisplayName = "Получение вложения бага с organizationId")]
    public async Task GetBugAttachmentAsync_WithOrganization_ShouldReturnAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var organizationId = $"org_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId, organizationId);
        var bug = await CreateTestBugAsync(userId, report.Id, organizationId);
        var attachment = await CreateTestBugAttachmentAsync(userId, bug.Id);

        // Act
        var result = await _attachmentDbClient.GetBugAttachmentAsync(organizationId, report.Id, bug.Id, attachment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachment.Id, result.Id);
    }

    #endregion

    #region GetCommentAttachment Tests

    [Fact(DisplayName = "Получение вложения комментария по ID")]
    public async Task GetCommentAttachmentAsync_ValidAttachment_ShouldReturnAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");
        var attachment = await CreateTestCommentAttachmentAsync(userId, comment.Id);

        // Act
        var result = await _attachmentDbClient.GetCommentAttachmentAsync(
            null, report.Id, bug.Id, comment.Id, attachment.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(attachment.Id, result.Id);
        Assert.Equal(attachment.FileName, result.FileName);
    }

    [Fact(DisplayName = "Получение несуществующего вложения комментария")]
    public async Task GetCommentAttachmentAsync_NonExistent_ShouldReturnNull()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");

        // Act
        var result = await _attachmentDbClient.GetCommentAttachmentAsync(
            null, report.Id, bug.Id, comment.Id, 999999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetBugAttachmentsCount Tests

    [Fact(DisplayName = "Подсчет вложений бага - нет вложений")]
    public async Task GetBugAttachmentsCountAsync_NoAttachments_ShouldReturnZero()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);

        // Act
        var count = await _attachmentDbClient.GetBugAttachmentsCountAsync(null, report.Id, bug.Id, AttachType_BugFact);

        // Assert
        Assert.Equal(0, count);
    }

    [Fact(DisplayName = "Подсчет вложений бага - несколько вложений")]
    public async Task GetBugAttachmentsCountAsync_MultipleAttachments_ShouldReturnCorrectCount()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        
        await CreateTestBugAttachmentAsync(userId, bug.Id);
        await CreateTestBugAttachmentAsync(userId, bug.Id);
        await CreateTestBugAttachmentAsync(userId, bug.Id);

        // Act
        var count = await _attachmentDbClient.GetBugAttachmentsCountAsync(null, report.Id, bug.Id, AttachType_BugFact);

        // Assert
        Assert.Equal(3, count);
    }

    #endregion

    #region GetCommentAttachmentsCount Tests

    [Fact(DisplayName = "Подсчет вложений комментария - нет вложений")]
    public async Task GetCommentAttachmentsCountAsync_NoAttachments_ShouldReturnZero()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");

        // Act
        var count = await _attachmentDbClient.GetCommentAttachmentsCountAsync(
            userId, null, report.Id, bug.Id, comment.Id);

        // Assert
        Assert.Equal(0, count);
    }

    [Fact(DisplayName = "Подсчет вложений комментария - несколько вложений")]
    public async Task GetCommentAttachmentsCountAsync_MultipleAttachments_ShouldReturnCorrectCount()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");
        
        await CreateTestCommentAttachmentAsync(userId, comment.Id);
        await CreateTestCommentAttachmentAsync(userId, comment.Id);

        // Act
        var count = await _attachmentDbClient.GetCommentAttachmentsCountAsync(
            userId, null, report.Id, bug.Id, comment.Id);

        // Assert
        Assert.Equal(2, count);
    }

    #endregion

    #region DeleteBugAttachment Tests

    [Fact(DisplayName = "Удаление вложения бага")]
    public async Task DeleteBugAttachmentAsync_ValidAttachment_ShouldDeleteAndReturnAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var attachment = await CreateTestBugAttachmentAsync(userId, bug.Id);

        // Act
        var deleted = await _attachmentDbClient.DeleteBugAttachmentAsync(null, report.Id, bug.Id, attachment.Id);

        // Assert
        Assert.NotNull(deleted);
        Assert.Equal(attachment.Id, deleted.Id);

        // Проверяем что вложение действительно удалено
        var getResult = await _attachmentDbClient.GetBugAttachmentAsync(null, report.Id, bug.Id, attachment.Id);
        Assert.Null(getResult);
    }

    [Fact(DisplayName = "Удаление несуществующего вложения бага")]
    public async Task DeleteBugAttachmentAsync_NonExistent_ShouldReturnNull()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);

        // Act
        var result = await _attachmentDbClient.DeleteBugAttachmentAsync(null, report.Id, bug.Id, 999999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region DeleteCommentAttachment Tests

    [Fact(DisplayName = "Удаление вложения комментария")]
    public async Task DeleteCommentAttachmentAsync_ValidAttachment_ShouldDeleteAndReturnAttachment()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");
        var attachment = await CreateTestCommentAttachmentAsync(userId, comment.Id);

        // Act
        var deleted = await _attachmentDbClient.DeleteCommentAttachmentAsync(
            null, report.Id, bug.Id, comment.Id, attachment.Id);

        // Assert
        Assert.NotNull(deleted);
        Assert.Equal(attachment.Id, deleted.Id);

        // Проверяем что вложение действительно удалено
        var getResult = await _attachmentDbClient.GetCommentAttachmentAsync(
            null, report.Id, bug.Id, comment.Id, attachment.Id);
        Assert.Null(getResult);
    }

    [Fact(DisplayName = "Удаление несуществующего вложения комментария")]
    public async Task DeleteCommentAttachmentAsync_NonExistent_ShouldReturnNull()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");

        // Act
        var result = await _attachmentDbClient.DeleteCommentAttachmentAsync(
            null, report.Id, bug.Id, comment.Id, 999999);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region DeleteCommentAttachments Tests

    [Fact(DisplayName = "Удаление всех вложений комментария")]
    public async Task DeleteCommentAttachmentsAsync_MultipleAttachments_ShouldDeleteAll()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");
        
        var attachment1 = await CreateTestCommentAttachmentAsync(userId, comment.Id);
        var attachment2 = await CreateTestCommentAttachmentAsync(userId, comment.Id);
        var attachment3 = await CreateTestCommentAttachmentAsync(userId, comment.Id);

        // Act
        var deleted = await _attachmentDbClient.DeleteCommentAttachmentsAsync(comment.Id);

        // Assert
        Assert.NotNull(deleted);
        Assert.Equal(3, deleted.Length);
        Assert.Contains(deleted, a => a.Id == attachment1.Id);
        Assert.Contains(deleted, a => a.Id == attachment2.Id);
        Assert.Contains(deleted, a => a.Id == attachment3.Id);
    }

    [Fact(DisplayName = "Удаление вложений комментария без вложений")]
    public async Task DeleteCommentAttachmentsAsync_NoAttachments_ShouldReturnEmpty()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);
        var comment = await CreateTestCommentAsync(userId, report.Id, bug.Id, "Test");

        // Act
        var deleted = await _attachmentDbClient.DeleteCommentAttachmentsAsync(comment.Id);

        // Assert
        Assert.NotNull(deleted);
        Assert.Empty(deleted);
    }

    #endregion

    #region Complex Workflow Tests

    [Fact(DisplayName = "Полный жизненный цикл вложения: создание -> обновление -> получение -> удаление")]
    public async Task AttachmentLifecycle_CreateUpdateGetDelete_ShouldWorkCorrectly()
    {
        // Arrange
        var userId = $"user_{Guid.NewGuid()}";
        var report = await CreateTestReportAsync(userId);
        var bug = await CreateTestBugAsync(userId, report.Id);

        // Act & Assert - Создание
        var created = await CreateTestBugAttachmentAsync(userId, bug.Id);
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

        // Act & Assert - Обновление
        var updateModel = new UpdateAttachmentDbModel
        {
            Id = created.Id,
            StorageKey = "updated_key.jpg",
            StorageKind = 2,
            LengthBytes = 5000,
            FileName = "updated.jpg",
            MimeType = "image/jpeg",
            HasPreview = true,
            IsGzipCompressed = false
        };
        var updated = await _attachmentDbClient.UpdateAttachmentAsync(updateModel);
        Assert.NotNull(updated);
        Assert.Equal("updated.jpg", updated.FileName);

        // Act & Assert - Получение
        var retrieved = await _attachmentDbClient.GetBugAttachmentAsync(null, report.Id, bug.Id, created.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);

        // Act & Assert - Удаление
        var deleted = await _attachmentDbClient.DeleteBugAttachmentAsync(null, report.Id, bug.Id, created.Id);
        Assert.NotNull(deleted);
        var afterDelete = await _attachmentDbClient.GetBugAttachmentAsync(null, report.Id, bug.Id, created.Id);
        Assert.Null(afterDelete);
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

    private async Task<AttachmentDbModel> CreateTestBugAttachmentAsync(string userId, int bugId)
    {
        var createModel = new CreateAttachmentDbModel
        {
            EntityId = bugId,
            AttachType = AttachType_BugFact,
            StorageKey = $"test/bug_{bugId}_{Guid.NewGuid()}.jpg",
            StorageKind = 1,
            CreatorUserId = userId,
            LengthBytes = 1024,
            FileName = "test_file.jpg",
            MimeType = "image/jpeg"
        };
        return await _attachmentDbClient.CreateAttachment(createModel);
    }

    private async Task<AttachmentDbModel> CreateTestCommentAttachmentAsync(string userId, int commentId)
    {
        var createModel = new CreateAttachmentDbModel
        {
            EntityId = commentId,
            AttachType = AttachType_Comment,
            StorageKey = $"test/comment_{commentId}_{Guid.NewGuid()}.jpg",
            StorageKind = 1,
            CreatorUserId = userId,
            LengthBytes = 2048,
            FileName = "test_comment_file.jpg",
            MimeType = "image/jpeg"
        };
        return await _attachmentDbClient.CreateAttachment(createModel);
    }

    #endregion
}


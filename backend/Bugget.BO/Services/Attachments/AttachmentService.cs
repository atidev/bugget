using Bugget.DA.Postgres;
using Bugget.Entities.Authentication;
using Bugget.Entities.BO;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.BO.Validators;
using Monade;
using TaskQueue;
using Bugget.Entities.DbModels.Attachment;
using Microsoft.Extensions.Logging;
using Bugget.BO.Errors;
using Bugget.DA.Interfaces;
using Bugget.BO.Interfaces;

namespace Bugget.BO.Services.Attachments;

public sealed class AttachmentService(
    AttachmentWriterService attachmentWriterService,
    AttachmentDbClient attachmentDbClient,
    ITaskQueue taskQueue,
    AttachmentEventsService attachmentEventsService,
    LimitsService limitsService,
    ILogger<AttachmentService> logger,
    IFileStorageClient fileStorageClient,
    IAttachmentKeyGenerator keyGen)
{
    public async Task<MonadeStruct<(Stream content, AttachmentDbModel attachmentDbModel)>> GetBugAttachmentContentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int attachmentId)
    {
        var attachmentDbModel = await attachmentDbClient.GetBugAttachmentAsync(user.OrganizationId, reportId, bugId, attachmentId);
        if (attachmentDbModel == null)
            return BoErrors.AttachmentNotFound;

        var content = await fileStorageClient.ReadAsync(attachmentDbModel.StorageKey);
        return (content, attachmentDbModel);
    }

    public async Task<MonadeStruct<(Stream content, AttachmentDbModel attachmentDbModel)>> GetCommentAttachmentContentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int commentId,
        int attachmentId)
    {
        var attachmentDbModel = await attachmentDbClient.GetCommentAttachmentAsync(user.OrganizationId, reportId, bugId, commentId, attachmentId);
        if (attachmentDbModel == null)
            return BoErrors.AttachmentNotFound;

        var content = await fileStorageClient.ReadAsync(attachmentDbModel.StorageKey);
        return (content, attachmentDbModel);
    }

    public async Task<MonadeStruct<Stream>> GetBugAttachmentPreviewContentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int attachmentId)
    {
        var attachmentDbModel = await attachmentDbClient.GetBugAttachmentAsync(user.OrganizationId, reportId, bugId, attachmentId);
        if (attachmentDbModel == null)
            return BoErrors.AttachmentNotFound;

        var content = await fileStorageClient.ReadAsync(keyGen.GetPreviewKey(attachmentDbModel.StorageKey));
        return content;
    }

    public async Task<MonadeStruct<Stream>> GetCommentAttachmentPreviewContentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int commentId,
        int attachmentId)
    {
        var attachmentDbModel = await attachmentDbClient.GetCommentAttachmentAsync(user.OrganizationId, reportId, bugId, commentId, attachmentId);
        if (attachmentDbModel == null)
            return BoErrors.AttachmentNotFound;

        var content = await fileStorageClient.ReadAsync(keyGen.GetPreviewKey(attachmentDbModel.StorageKey));
        return content;
    }

    public async Task DeleteBugAttachmentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int attachmentId)
    {
        var attachmentDbModel = await attachmentDbClient.DeleteBugAttachmentAsync(user.OrganizationId, reportId, bugId, attachmentId);
        if (attachmentDbModel == null)
            return;

        await taskQueue.EnqueueAsync(async () =>
            await attachmentEventsService.HandleAttachmentDeleteEventAsync(reportId, user, attachmentDbModel));

        return;
    }

    public async Task DeleteCommentAttachmentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int commentId,
        int attachmentId)
    {
        var attachmentDbModel = await attachmentDbClient.DeleteCommentAttachmentAsync(user.OrganizationId, reportId, bugId, commentId, attachmentId);
        if (attachmentDbModel == null)
            return;

        await taskQueue.EnqueueAsync(async () =>
            await attachmentEventsService.HandleAttachmentDeleteEventAsync(reportId, user, attachmentDbModel));

        return;
    }

    public Task<MonadeStruct<AttachmentDbModel>> SaveBugAttachmentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        Stream fileStream,
        AttachType attachType,
        FileMeta fileMeta)
    {

        Task<Error?> validateLimit() => limitsService.ValidateBugAttachmentLimitAsync(user, reportId, bugId, attachType);

        return SaveAsync(
            user: user,
            reportId: reportId,
            entityId: bugId,
            validateLimit: validateLimit,
            fileStream: fileStream,
            attachType: attachType,
            fileMeta: fileMeta);
    }

    public Task<MonadeStruct<AttachmentDbModel>> SaveCommentAttachmentAsync(
        UserIdentity user,
        int reportId,
        int bugId,
        int commentId,
        Stream fileStream,
        FileMeta fileMeta)
    {
        Task<Error?> validateLimit() => limitsService.ValidateCommentAttachmentLimitAsync(user, reportId, bugId, commentId);

        return SaveAsync(
            user: user,
            reportId: reportId,
            entityId: commentId,
            validateLimit: validateLimit,
            fileStream: fileStream,
            attachType: AttachType.Comment,
            fileMeta: fileMeta);
    }

    private async Task<MonadeStruct<AttachmentDbModel>> SaveAsync(
        UserIdentity user,
        int reportId,
        int entityId,
        Func<Task<Error?>> validateLimit,
        Stream fileStream,
        AttachType attachType,
        FileMeta fileMeta)
    {
        // 1) Общая валидация по метаданным
        var validationError = AttachmentValidator.Validate(fileMeta);
        if (validationError != null)
            return validationError;

        // 2) Лимиты (замыкание подставит нужный метод)
        var limitError = await validateLimit();
        if (limitError != null)
            return limitError;

        // 3) Process + сохранение файла
        var saveResult = await attachmentWriterService.ProcessAsync(
            user.OrganizationId,
            reportId,
            entityId,
            fileStream,
            fileMeta,
            CancellationToken.None);

        logger.LogInformation("Attachment saved: {@FileName}, compress score {@from}-{@to}-{@preview} percent {@percent}%",
        fileMeta.FileName, fileStream.Length, saveResult.LengthBytes, saveResult.PreviewLengthBytes, (1 - (double)saveResult.LengthBytes / fileStream.Length) * 100);

        // 4) Формируем модель для БД
        var createModel = new AttachmentCreateDbModel
        {
            EntityId = entityId,
            AttachType = (int)attachType,
            StorageKey = saveResult.StorageKey,
            StorageKind = (int)StorageKind.Local,
            CreatorUserId = user.Id,
            LengthBytes = saveResult.LengthBytes + saveResult.PreviewLengthBytes,
            FileName = fileMeta.FileName,
            MimeType = saveResult.MimeType,
            HasPreview = saveResult.HasPreview,
            IsGzipCompressed = saveResult.IsGzipCompressed
        };

        // 5) Сохраняем в БД
        var dbModel = await attachmentDbClient.CreateAttachment(createModel);

        // 6) Событие
        await taskQueue.EnqueueAsync(async () =>
            await attachmentEventsService.HandleAttachmentCreateEventAsync(reportId, user, dbModel));

        return dbModel;
    }
}
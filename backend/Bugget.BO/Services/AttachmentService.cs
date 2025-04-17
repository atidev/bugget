using Bugget.DA.Postgres;
using Bugget.Entities.BO;
using Bugget.Entities.Config;
using Bugget.Entities.DbModels;
using Bugget.Entities.Mappers;
using Microsoft.Extensions.Options;
using Monade;

namespace Bugget.BO.Services;

public sealed class AttachmentService(
    BugsService bugService,
    AttachmentDbClient attachmentDbClient,
    IOptions<FileStorageConfiguration> fileStorageConfigurationOptions)
{
    private readonly string _baseDir = fileStorageConfigurationOptions.Value.BaseDirectory.TrimEnd('/');

    public async Task<(byte[] Content, string FileName)> GetAttachmentContent(int attachmentId)
    {
        var attachment = await GetAttachment(attachmentId);
        var absolutePath = GetAttachmentContentAbsolutePath(attachment.Path);
        if (!File.Exists(absolutePath))
        {
            throw new Exception("Attachment content not found");
        }

        return (await File.ReadAllBytesAsync(absolutePath), Path.GetFileName(absolutePath));
    }

    public async Task DeleteAttachment(int attachmentId)
    {
        var attachment = await GetAttachment(attachmentId);
        var absolutePath = GetAttachmentContentAbsolutePath(attachment.Path);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        await attachmentDbClient.DeleteAttachment(attachmentId);
    }
    public async Task<MonadeStruct<Attachment[]>> SaveAttachments(int reportId, int bugId, string? organizationId, IEnumerable<(Stream Stream, AttachType AttachType, string FileName)> streamsWithMetadata)
    {
        var bugDbModel = await bugService.GetBugSummaryAsync(reportId, bugId, organizationId);
        if (bugDbModel.HasError)
        {
            return bugDbModel.Error!;
        }

        var concurrencyLevel = 5;
        var saveContentTasks = new Task<MonadeStruct<Attachment>>[concurrencyLevel];
        foreach (var streamsChunk in streamsWithMetadata.Chunk(concurrencyLevel))
        {
            for (int i = 0; i < concurrencyLevel; i++)
                saveContentTasks[i] = SaveAttachment(
                    reportId, bugDbModel.Value!.Id, organizationId, streamsChunk[i].Stream, streamsChunk[i].AttachType, streamsChunk[i].FileName);

            await Task.WhenAll(saveContentTasks);
        }

        return saveContentTasks.Where(t => t.Result.IsSuccess).Select(t => t.Result.Value!).ToArray();
    }

    public async Task<MonadeStruct<Attachment>> SaveAttachment(int reportId, int bugId, string? organizationId, Stream fileStream, AttachType attachType, string fileName)
    {
        var bugDbModel = await bugService.GetBugSummaryAsync(reportId, bugId, organizationId);
        if (bugDbModel.HasError)
        {
            return bugDbModel.Error!;
        }
        return await SaveAttachment(reportId, bugDbModel.Value!.Id, fileStream, attachType, fileName);
    }

    private async Task<Attachment> SaveAttachment(int reportId, int bugId, Stream fileStream, AttachType attachType, string fileName)
    {
        var relativePath = GetAttachmentFilePath(reportId, bugId, fileName);
        var fullPath = GetAttachmentContentAbsolutePath(relativePath);

        if (await attachmentDbClient.FilePathExist(relativePath))
        {
            // todo: error
            throw new Exception("Attachment save failed");
        }

        var dirPath = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath!);

        await using (var writeFs = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(writeFs);
        }

        var dbModel = new AttachmentDbModel()
        {
            AttachType = (int)attachType,
            BugId = bugId,
            CreatedAt = DateTimeOffset.UtcNow,
            Path = relativePath
        };
        var attachment = await attachmentDbClient.CreateAttachment(dbModel);
        if (attachment == null)
        {
            throw new Exception("Attachment save failed");
        }

        return attachment.ToAttachment();
    }

    private string GetAttachmentFilePath(int reportId, int bugId, string fileName) =>
        $"{reportId}/{bugId}/{fileName.Trim('.')}";

    private string GetAttachmentContentAbsolutePath(string attachmentRelativePath)
        => Path.Combine(_baseDir, attachmentRelativePath);

    private async Task<AttachmentDbModel> GetAttachment(int attachmentId)
    {
        var attachment = await attachmentDbClient.GetAttachment(attachmentId);
        if (attachment == null)
        {
            throw new Exception("Attachment not found");
        }

        return attachment;
    }
}
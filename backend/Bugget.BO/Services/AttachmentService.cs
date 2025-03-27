using AutoMapper;
using Bugget.DA.Postgres;
using Bugget.Entities.BO;
using Bugget.Entities.Config;
using Bugget.Entities.DbModels;
using Microsoft.Extensions.Options;

namespace Bugget.BO.Services;

public sealed class AttachmentService(
    BugsService bugService, 
    AttachmentDbClient attachmentDbClient,
    IOptions<FileStorageConfiguration> fileStorageConfigurationOptions,
    IMapper mapper)
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
    public async Task<IEnumerable<Attachment>> SaveAttachments(int bugId, string userId, IEnumerable<(Stream Stream, AttachType AttachType, string FileName)> streamsWithMetadata)
    {
        var bug = await bugService.GetBug(bugId);
        var concurrencyLevel = 5;
        var saveContentTasks = new Task<Attachment>[concurrencyLevel];
        foreach (var streamsChunk in streamsWithMetadata.Chunk(concurrencyLevel))
        {
            for (int i = 0; i < concurrencyLevel; i++)
                saveContentTasks[i] = SaveAttachment(
                    bug, userId, streamsChunk[i].Stream, streamsChunk[i].AttachType, streamsChunk[i].FileName);
            
            await Task.WhenAll(saveContentTasks);
        }

        return saveContentTasks.Select(t => t.Result).ToList();
    }

    public async Task<Attachment> SaveAttachment(int bugId, string userId, Stream fileStream, AttachType attachType, string fileName)
    {
        var bug = await bugService.GetBug(bugId);
        return await SaveAttachment(bug, userId, fileStream, attachType, fileName);
    }

    private async Task<Attachment> SaveAttachment(Bug bug, string userId, Stream fileStream, AttachType attachType, string fileName)
    {
        var relativePath = GetAttachmentFilePath(bug, fileName);
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
            BugId = bug.Id!.Value,
            CreatedAt = DateTimeOffset.UtcNow,
            Path = relativePath
        };
        var attachment = await attachmentDbClient.CreateAttachment(dbModel);
        if (attachment == null)
        {
            throw new Exception("Attachment save failed");
        }

        return mapper.Map<Attachment>(attachment);
    }

    private string GetAttachmentFilePath(Bug bug, string fileName) =>
        $"{bug.ReportId}/{bug.Id}/{fileName.Trim('.')}";

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
using AutoMapper;
using Bugget.DA.Postgres;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.Options;
using Microsoft.Extensions.Options;

namespace Bugget.BO.Services;

public sealed class AttachmentObsoleteService(
    BugsService bugService, 
    AttachmentObsoleteDbClient attachmentDbClient,
    IOptions<FileStorageOptions> fileStorageOptions,
    IMapper mapper)
{
    private readonly string _baseDir = fileStorageOptions.Value.BaseDirectory.TrimEnd('/');

    public async Task<(byte[] Content, string FileName)> GetAttachmentContentObsoleteAsync(int attachmentId)
    {
        var attachment = await GetAttachment(attachmentId);
        var absolutePath = GetAttachmentContentAbsolutePath(attachment.Path);
        if (!File.Exists(absolutePath))
        {
            throw new Exception("Attachment content not found");
        }

        return (await File.ReadAllBytesAsync(absolutePath), Path.GetFileName(absolutePath));
    }

    public async Task DeleteAttachmentObsoleteAsync(int attachmentId)
    {
        var attachment = await GetAttachment(attachmentId);
        var absolutePath = GetAttachmentContentAbsolutePath(attachment.Path);
        if (File.Exists(absolutePath))
        {
            File.Delete(absolutePath);
        }

        await attachmentDbClient.DeleteAttachment(attachmentId);
    }

    public async Task<Attachment> SaveAttachmentObsoleteAsync(int bugId, string userId, Stream fileStream, AttachType attachType, string fileName)
    {
        var bug = await bugService.GetBug(bugId);
        return await SaveAttachmentObsoleteAsync(bug, fileStream, attachType, fileName);
    }

    private async Task<Attachment> SaveAttachmentObsoleteAsync(Bug bug, Stream fileStream, AttachType attachType, string fileName)
    {
        var relativePath = GetAttachmentFilePathObsolete(bug);
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
        
        var dbModel = new AttachmentDbModel
        {
            AttachType = (int)attachType,
            BugId = bug.Id!.Value,
            CreatedAt = DateTimeOffset.UtcNow,
            Path = relativePath,
            Id = 0
        };
        var attachment = await attachmentDbClient.CreateAttachment(dbModel);
        if (attachment == null)
        {
            throw new Exception("Attachment save failed");
        }

        return mapper.Map<Attachment>(attachment);
    }

    private string GetAttachmentFilePathObsolete(Bug bug) =>
        $"{bug.ReportId}/{bug.Id}/{Guid.NewGuid()}";

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
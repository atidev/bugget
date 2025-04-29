using Authentication;
using AutoMapper;
using Bugget.BO.Services;
using Bugget.Entities.BO;
using Bugget.Entities.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Bugget.Controllers;

[Auth]
[Route("/v1/reports/{reportId}/bug/{bugId}/attachments")]
public sealed class AttachmentsController(AttachmentService attachmentService) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateAttachment(
        [FromRoute] int reportId,
        int bugId,
        [FromQuery(Name = "attachType")] int attachType,
        IFormFile file
        )
    {
        await using var stream = file.OpenReadStream();
        var attachment = await attachmentService.SaveAttachment(
            bugId, User.GetIdentity().Id, stream, (AttachType)attachType, file.FileName);
        return Ok(new AttachmentView
        {
            Id = attachment.Id!.Value,
            BugId = attachment.BugId,
            ReportId = reportId,
            Path = attachment.Path,
            CreatedAt = attachment.CreatedAt!.Value,
            AttachType = attachment.AttachType
        });
    }

    [HttpGet("{id}/content")]
    public async Task<FileContentResult> GetAttachmentContent(int id)
    {
        var (content, fileName) = await attachmentService.GetAttachmentContent(id);
        return new FileContentResult(content, GetMimeTypeForFileExtension(fileName));
    }
    
    [HttpDelete("{id}")]
    public Task DeleteAttachmentContent(int id)
    {
        return attachmentService.DeleteAttachment(id);
    }

    private string GetMimeTypeForFileExtension(string filePath)
    {
        const string DefaultContentType = "application/octet-stream";

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(filePath, out string contentType))
        {
            contentType = DefaultContentType;
        }

        return contentType;
    }
}
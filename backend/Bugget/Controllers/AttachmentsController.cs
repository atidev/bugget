using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.Entities.BO;
using Bugget.Entities.Views;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Bugget.Controllers;

[LdapAuth]
[Route("bugget/public/v1/reports/{reportId}/bug/{bugId}/attachments")]
public sealed class AttachmentsController(AttachmentService attachmentService) : ApiController
{
    [HttpPost]
    public async Task<IResult> CreateAttachment(
        [FromRoute] int reportId,
        int bugId,
        [FromQuery(Name = "attachType")] int attachType,
        IFormFile file
        )
    {
        await using var stream = file.OpenReadStream();
        var user = User.GetIdentity();
        return await attachmentService.SaveAttachment(
            reportId, bugId, user.OrganizationId, stream, (AttachType)attachType, file.FileName)
            .AsResultAsync((a) => new AttachmentView
            {
                Id = a.Id!.Value,
                BugId = a.BugId,
                ReportId = reportId,
                Path = a.Path,
                CreatedAt = a.CreatedAt!.Value,
                AttachType = a.AttachType
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
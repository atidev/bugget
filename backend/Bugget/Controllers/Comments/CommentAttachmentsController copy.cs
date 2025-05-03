using Bugget.BO.Services.Attachments;
using Bugget.Entities.Authentication;
using Bugget.Entities.BO;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.Constants;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

[Route("/v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}/attachments")]
public sealed class CommentAttachmentsController(AttachmentService attachmentService) : ApiController
{
    [HttpPost]
    [ProducesResponseType(typeof(AttachmentDbModel), 200)]
    public async Task<IActionResult> CreateAttachment(
        [FromRoute] int reportId,
        [FromRoute] int bugId,
        [FromRoute] int commentId,
        IFormFile file
        )
    {
        await using var stream = file.OpenReadStream();
        return await attachmentService.SaveCommentAttachmentAsync(
            User.GetIdentity(),
            reportId,
            bugId,
            commentId,
            stream,
            new FileMeta(file.FileName, file.Length, file.ContentType)).AsActionResultAsync();
    }

    [HttpGet("{id}/content")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    public async Task<IActionResult> GetAttachmentContentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromRoute] int commentId, [FromRoute] int id)
    {
        var user = User.GetIdentity();
        var attachResult = await attachmentService.GetCommentAttachmentContentAsync(user, reportId, bugId, commentId, id);
        if (attachResult.HasError)
            return attachResult.AsActionResult();

        var (content, attachmentDbModel) = attachResult.Value;
        if (attachmentDbModel.IsGzipCompressed.Value)
        {
            Response.Headers["Content-Encoding"] = "gzip";
        }
        return new FileStreamResult(content, attachmentDbModel.MimeType);
    }

    [HttpGet("{id}/content/preview")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    public async Task<IActionResult> GetAttachmentPreviewContentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromRoute] int commentId, [FromRoute] int id)
    {
        var user = User.GetIdentity();
        var attachResult = await attachmentService.GetCommentAttachmentPreviewContentAsync(user, reportId, bugId, commentId, id);
        if (attachResult.HasError)
            return attachResult.AsActionResult();

        return new FileStreamResult(attachResult.Value!, AttachmentConstants.PreviewMimeType);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    public Task DeleteAttachmentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromRoute] int commentId, [FromRoute] int id)
    {
        var user = User.GetIdentity();
        return attachmentService.DeleteCommentAttachmentAsync(user, reportId, bugId, commentId, id);
    }
}
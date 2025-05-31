using Bugget.BO.Mappers;
using Bugget.BO.Services.Attachments;
using Bugget.Entities.Authentication;
using Bugget.Entities.BO;
using Bugget.Entities.BO.AttachmentBo;
using Bugget.Entities.Constants;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.Views.Attachment;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Bugget.Controllers;

[Route("/v2/reports/{reportId}/bugs/{bugId}/comments/{commentId}/attachments")]
public sealed class CommentAttachmentsController(AttachmentService attachmentService) : ApiController
{
    private static bool IsDevelopment =
    Environment.GetEnvironmentVariable(EnvironmentConstants.AspnetcoreEnvironment)?
        .Equals("development", StringComparison.OrdinalIgnoreCase) ?? false;


    [HttpPost]
    [ProducesResponseType(typeof(AttachmentView), 201)]
    public async Task<IActionResult> CreateAttachment(
        [FromRoute] int reportId,
        [FromRoute] int bugId,
        [FromRoute] int commentId,
        IFormFile file,
        CancellationToken ct
        )
    {
        // Открываем поток. IFormFile.OpenReadStream обычно CanSeek == true,
        // но если вдруг нет, то оборачиваем в FileBufferingReadStream:
        Stream fileStream = file.OpenReadStream();
        if (!fileStream.CanSeek)
        {
            fileStream = new FileBufferingReadStream(
                HttpContext.Request.Body,
                _ = 1024 * 1024,    // threshold before disk
                _ = 8 * 1024,         // buffer size
                _ = Path.GetTempPath()
            );
            // Переписать файл из Request.Body в наш буфер:
            await file.CopyToAsync(fileStream, ct);
            fileStream.Position = 0;
        }

        // Детектим MIME
        var mimeType = IsDevelopment ? file.ContentType : await MimeHelper.GuessMimeAsync(fileStream, ct);

        return await attachmentService.SaveCommentAttachmentAsync(
            User.GetIdentity(),
            reportId,
            bugId,
            commentId,
            fileStream,
            new FileMeta(file.FileName, file.Length, mimeType),
            ct)
            .AsActionResultAsync((attachmentDbModel) => AttachmentMapper.ToView(attachmentDbModel, reportId), 201);
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
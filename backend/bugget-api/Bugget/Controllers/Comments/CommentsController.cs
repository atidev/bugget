using Bugget.BO.Services.Comments;
using Bugget.Entities.Authentication;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DTO.Comment;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers.Comments;

/// <summary>
/// Api для работы с комментами
/// </summary>
[Route("/v2/reports/{reportId}/bugs/{bugId}/comments")]
public sealed class CommentsController(
    CommentsService commentsService) : ApiController
{
    /// <summary>
    /// Добавить комментарий
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CommentSummaryDbModel), 200)]
    public Task<CommentSummaryDbModel> CreateCommentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromBody] CommentDto createDto)
    {
        var user = User.GetIdentity();
        return commentsService.CreateCommentAsync(user, reportId, bugId, createDto);
    }

    /// <summary>
    /// Удалить свой комментарий
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <param name="commentId"></param>
    /// <returns></returns>
    [HttpDelete("{commentId}")]
    [ProducesResponseType(200)]
    public Task DeleteCommentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromRoute] int commentId)
    {
        var user = User.GetIdentity();
        return commentsService.DeleteCommentAsync(user, reportId, bugId, commentId);
    }

    /// <summary>
    /// Обновить свой комментарий
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <param name="commentId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{commentId}")]
    [ProducesResponseType(typeof(CommentSummaryDbModel), 200)]
    public Task<IActionResult> UpdateCommentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromRoute] int commentId, [FromBody] CommentDto updateDto)
    {
        var user = User.GetIdentity();
        return commentsService.UpdateCommentAsync(user, reportId, bugId, commentId, updateDto).AsActionResultAsync();
    }   
}
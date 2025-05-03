using Bugget.BO.Mappers;
using Bugget.BO.Services;
using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.DTO;
using Bugget.Entities.DTO.Comment;
using Bugget.Entities.Views;
using Bugget.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с комментами
/// </summary>
[Route("/v1/reports/{reportId}/bugs/{bugId}/comments")]
[Obsolete("Используй CommentsV2Controller")]
public sealed class CommentsObsoleteController(
    CommentsObsoleteService commentsService,
    IHubContext<ReportPageHub> hubContext,
    IEmployeesClient employeesClient) : ApiController
{
    /// <summary>
    /// Добавить комментарий
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(CommentView), 200)]
    public async Task<IActionResult> CreateCommentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromBody] CommentObsoleteDto createDto)
    {
        var user = User.GetIdentity();
        var comment = await commentsService.CreateCommentAsync(createDto.ToComment(bugId, reportId, user.Id));
        
        if (comment == null)
        {
            return BadRequest("Не удалось создать комментарий.");
        }

        // Уведомляем всех подписчиков группы (reportId)
        await hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveComments", comment.BugId);
        
        return Ok(comment?.ToCommentView(employeesClient.DictEmployees()));
    }

    /// <summary>
    /// Получить все комментарии (для веб-сокета)
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(CommentView[]), 200)]
    public async Task<CommentView[]> ListCommentsAsync([FromRoute] int reportId, [FromRoute] int bugId)
    {
        var comments = await commentsService.ListCommentsAsync(reportId, bugId);
        return comments.Select(c => c.ToCommentView(employeesClient.DictEmployees())).ToArray();
    }

    /// <summary>
    /// Удалить свой комментарий
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <param name="commentId"></param>
    /// <returns></returns>
    [HttpDelete("{commentId}")]
    [ProducesResponseType( 200)]
    public Task DeleteCommentAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromRoute] int commentId)
    {
        var user = User.GetIdentity();
        return commentsService.DeleteCommentAsync(user.Id, reportId, bugId, commentId);
    }
}
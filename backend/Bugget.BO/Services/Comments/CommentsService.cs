using Bugget.BO.Errors;
using Bugget.DA.Postgres;
using Bugget.Entities.Authentication;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DTO.Comment;
using Monade;
using TaskQueue;

namespace Bugget.BO.Services.Comments;

public sealed class CommentsService(CommentsDbClient commentsDbClient, CommentEventsService commentEventsService, ITaskQueue taskQueue)
{
    public async Task<CommentSummaryDbModel> CreateCommentAsync(UserIdentity user, int reportId, int bugId, CommentDto commentDto)
    {
        var commentDbModel = await commentsDbClient.CreateCommentAsync(user.OrganizationId, user.Id, reportId, bugId, commentDto.Text);

        await taskQueue.EnqueueAsync(async () => await commentEventsService.HandleCommentCreateEventAsync(reportId, user, commentDbModel));

        return commentDbModel;
    }
    
    public async Task DeleteCommentAsync(UserIdentity user, int reportId, int bugId, int commmentId)
    {
        await commentsDbClient.DeleteCommentAsync(user.OrganizationId, user.Id, reportId, bugId, commmentId);

        await taskQueue.EnqueueAsync(async () => await commentEventsService.HandleCommentDeleteEventAsync(reportId, user, bugId, commmentId));
    }

    public async Task<MonadeStruct<CommentSummaryDbModel>> UpdateCommentAsync(UserIdentity user, int reportId, int bugId, int commmentId, CommentDto commentDto)
    {
        var commentDbModel = await commentsDbClient.UpdateCommentAsync(user.OrganizationId, user.Id, reportId, bugId, commmentId, commentDto.Text);

        if (commentDbModel == null)
        {
            return BoErrors.UserCommentNotFound;
        }

        await taskQueue.EnqueueAsync(async () => await commentEventsService.HandleCommentUpdateEventAsync(reportId, user, commentDbModel));

        return commentDbModel;
    }
}
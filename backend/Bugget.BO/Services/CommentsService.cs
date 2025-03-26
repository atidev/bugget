using Bugget.BO.Mappers;
using Bugget.DA.Postgres;
using Bugget.Entities.BO;
using Bugget.Entities.BO.BugBo;
using Bugget.Entities.DbModels.Comment;

namespace Bugget.BO.Services;

public sealed class CommentsService(CommentsDbClient commentsDbClient)
{
    public Task<CommentDbModel?> CreateCommentAsync(Comment comment)
    {
        return commentsDbClient.CreateCommentAsync(comment.ToCommentCreateDbModel());
    }

    public Task<CommentDbModel[]> ListCommentsAsync(int reportId, int bugId)
    {
        return commentsDbClient.ListCommentsAsync(reportId, bugId);
    }
    
    public Task DeleteCommentAsync(string userId, int reportId, int bugId, int commmentId)
    {
        return commentsDbClient.DeleteCommentAsync(userId, reportId, bugId, commmentId);
    }
}
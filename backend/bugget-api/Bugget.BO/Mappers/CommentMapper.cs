using Bugget.Entities.Adapters;
using Bugget.Entities.BO;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DTO.Comment;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Attachment;

namespace Bugget.BO.Mappers;

public static class CommentMapper
{
    public static CommentCreateDbModel ToCommentCreateDbModel(this Comment comment)
    {
        return new CommentCreateDbModel
        {
            CreatorUserId = comment.CreatorUserId,
            BugId = comment.BugId,
            ReportId = comment.ReportId,
            Text = comment.Text
        };
    }
}
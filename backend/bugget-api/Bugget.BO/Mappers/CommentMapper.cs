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

    public static CommentView ToCommentView(this CommentDbModel comment, int reportId, IReadOnlyDictionary<string, User> usersDict)
    {
        return new CommentView
        {
            Id = comment.Id,
            Creator = usersDict.TryGetValue(comment.CreatorUserId, out var e)
                ? UsersAdapter.ToUserView(e)
                : UsersAdapter.ToUserView(comment.CreatorUserId),

            BugId = comment.BugId,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Attachments = comment.Attachments?.Select(a => new AttachmentView
            {
                Id = a.Id,
                EntityId = a.EntityId!.Value,
                AttachType = a.AttachType,
                CreatedAt = a.CreatedAt,
                HasPreview = a.HasPreview.Value,
                FileName = a.FileName ?? string.Empty,
                CreatorUserId = a.CreatorUserId ?? string.Empty,
            }).ToArray()
        };
    }
}
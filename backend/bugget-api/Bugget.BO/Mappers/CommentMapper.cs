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

    public static Comment ToComment(this CommentObsoleteDto comment, int bugId, int reportId, string userId)
    {
        return new Comment
        {
            CreatorUserId = userId,
            BugId = bugId,
            ReportId = reportId,
            Text = comment.Text,
        };
    }

    public static CommentView ToCommentView(this CommentDbModel comment, int reportId, IReadOnlyDictionary<string, Employee> employeesDict)
    {
        return new CommentView
        {
            Id = comment.Id,
            Creator = employeesDict.TryGetValue(comment.CreatorUserId, out var e)
                ? EmployeeAdapter.ToUserView(e)
                : EmployeeAdapter.ToUserView(comment.CreatorUserId),

            BugId = comment.BugId,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Attachments = comment.Attachments?.Select(a => new AttachmentView
            {
                Id = a.Id,
                ReportId = reportId,
                BugId = comment.BugId,
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
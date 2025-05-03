using Bugget.Entities.Adapters;
using Bugget.Entities.BO;
using Bugget.Entities.Constants;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DTO;
using Bugget.Entities.DTO.Comment;
using Bugget.Entities.Views;

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

    public static CommentView ToCommentView(this CommentDbModel comment, IReadOnlyDictionary<string, Employee> employeesDict)
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
            UpdatedAt = comment.UpdatedAt
        };
    }
}
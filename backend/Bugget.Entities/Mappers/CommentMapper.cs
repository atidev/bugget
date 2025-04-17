using Bugget.Entities.BO;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DTO;
using Bugget.Entities.Views;

namespace Bugget.Entities.Mappers;

public static class CommentMapper
{
    public static CommentCreateDbModel ToCommentCreateDbModel(this CommentDto comment, string userId, int bugId, int reportId)
    {
        return new CommentCreateDbModel
        {
            ReportId = reportId,
            CreatorUserId = userId,
            BugId = bugId,
            Text = comment.Text
        };
    }

    public static Comment ToComment(this CommentDto comment, int bugId, int reportId, string userId)
    {
        return new Comment
        {
            CreatorUserId = userId,
            BugId = bugId,
            ReportId = reportId,
            Text = comment.Text,
        };
    }

    public static CommentViewObsolete ToCommentViewObsolete(this CommentDbModel comment, IReadOnlyDictionary<string, EmployeeObsolete> employeesDict)
    {
        return new CommentViewObsolete
        {
            Id = comment.Id,
            Creator = employeesDict.TryGetValue(comment.CreatorUserId, out var e)
                ? EmployeesMapper.ToUserView(e)
                : EmployeesMapper.ToUserView(comment.CreatorUserId),
            BugId = comment.BugId,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public static CommentView ToCommentView(this CommentDbModel comment)
    {
        return new CommentView
        {
            Id = comment.Id,
            CreatorUserId = comment.CreatorUserId,
            BugId = comment.BugId,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}
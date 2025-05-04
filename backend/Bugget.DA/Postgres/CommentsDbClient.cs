using System.Text.Json;
using Bugget.Entities.DbModels.Comment;
using Dapper;

namespace Bugget.DA.Postgres;

public sealed class CommentsDbClient : PostgresClient
{
    public async Task<CommentSummaryDbModel> CreateCommentAsync(string? organizationId, string userId, int reportId, int bugId, string text)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleAsync<CommentSummaryDbModel>(
            "SELECT * FROM public.create_comment_v2(@organization_id, @user_id, @report_id, @bug_id, @text);",
            new
            {
                organization_id = organizationId,
                        user_id = userId,
                        report_id = reportId,
                        bug_id = bugId,
                        text = text,
                    }
                );

    }

    public async Task DeleteCommentAsync(string? organizationId, string userId, int reportId, int bugId, int commentId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        await connection.ExecuteScalarAsync(
            "SELECT public.delete_comment_v2(@organization_id, @user_id, @report_id, @bug_id, @comment_id);",
            new
            {
                organization_id = organizationId,
                user_id = userId,
                report_id = reportId,
                bug_id = bugId,
                comment_id = commentId
            }
        );
    }

    public async Task<CommentSummaryDbModel?> UpdateCommentAsync(string? organizationId, string userId, int reportId, int bugId, int commentId, string text)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleOrDefaultAsync<CommentSummaryDbModel>(
            "SELECT * FROM public.update_comment_v2(@organization_id, @user_id, @report_id, @bug_id, @comment_id, @text);",
            new
            {
                organization_id = organizationId,
                user_id = userId,
                report_id = reportId,
                bug_id = bugId,
                comment_id = commentId,
                text = text
            }
        );
    }
}
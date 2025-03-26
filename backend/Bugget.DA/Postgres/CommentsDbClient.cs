using System.Text.Json;
using System.Text.Json.Serialization;
using Bugget.Entities.Config;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DbModels.Comment;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Bugget.DA.Postgres;

public sealed class CommentsDbClient: PostgresClient
{
    /// <summary>
    /// Создает новый отчет и возвращает его полную структуру.
    /// </summary>
    public async Task<CommentDbModel?> CreateCommentAsync(CommentCreateDbModel commentCreateDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.create_comment(@report_id, @bug_id, @text, @creator_user_id);",
            new
            {
                report_id = commentCreateDbModel.ReportId,
                bug_id = commentCreateDbModel.BugId,
                text = commentCreateDbModel.Text,
                creator_user_id = commentCreateDbModel.CreatorUserId,
            }
        );

        return jsonResult != null
            ? Deserialize<CommentDbModel>(jsonResult)
            : null;
    }

    public async Task<CommentDbModel[]> ListCommentsAsync(int reportId, int bugId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResults = await connection.QueryAsync<string>(
            "SELECT public.list_comments(@report_id, @bug_id);",
            new { report_id = reportId, bug_id = bugId }
        );

        return jsonResults
            .Where(json => json != null)
            .Select(json => Deserialize<CommentDbModel>(json)!)
            .ToArray();
    }
    
    public async Task DeleteCommentAsync(string userId, int reportId, int bugId, int commentId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        await connection.ExecuteAsync(
            "CALL public.delete_comment(@user_id, @report_id, @bug_id, @comment_id);",
            new { user_id = userId, report_id = reportId, bug_id = bugId, comment_id = commentId }
        );
    }

    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
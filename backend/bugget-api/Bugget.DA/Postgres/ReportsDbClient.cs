using System.Runtime.CompilerServices;
using System.Text.Json;
using Bugget.Entities.BO;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Attachment;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DbModels.Comment;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Dapper;

namespace Bugget.DA.Postgres;

public sealed class ReportsDbClient : PostgresClient
{
    /// <summary>
    /// Получает отчет по ID.
    /// </summary>
    public async Task<ReportObsoleteDbModel?> GetReportAsync(int reportId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.get_report(@report_id);",
            new { report_id = reportId }
        );

        return jsonResult != null ? Deserialize<ReportObsoleteDbModel>(jsonResult) : null;
    }

    /// <summary>
    /// Получает отчет по ID.
    /// </summary>
    public async Task<ReportDbModel?> GetReportAsync(int reportId, string? organizationId)
    {
        await using var conn = await DataSource.OpenConnectionAsync();
        // Открываем транзакцию для курсоров
        await using var multi = await conn.QueryMultipleAsync(@"
  SELECT * FROM public.get_report_v2(@reportId, @organizationId);
  SELECT * FROM public.list_bugs_internal(@reportId);
  SELECT * FROM public.list_participants_internal(@reportId);
  SELECT * FROM public.list_comments_internal(@reportId);
  SELECT * FROM public.list_attachments_internal(@reportId);
", new { reportId, organizationId });

        // проверка доступа к репорту
        var report = await multi.ReadSingleOrDefaultAsync<ReportDbModel>();
        if (report == null) return null;

        // 2. Дочерние сущности
        report.Bugs = (await multi.ReadAsync<BugDbModel>()).ToArray();
        report.ParticipantsUserIds = (await multi.ReadAsync<string>()).ToArray();
        var comments = (await multi.ReadAsync<CommentDbModel>()).ToArray();
        var attachments = (await multi.ReadAsync<AttachmentDbModel>()).ToArray();

        // 3. Группируем по багам
        var commentsByBug = comments.GroupBy(c => c.BugId).ToDictionary(g => g.Key, g => g.OrderBy(c => c.CreatedAt).ToArray());
        var attachmentsByEntity = attachments.GroupBy(a => a.EntityId).ToDictionary(g => g.Key, g => g.OrderBy(a => a.CreatedAt).ToArray());

        foreach (var bug in report.Bugs)
        {
            bug.Comments = commentsByBug.TryGetValue(bug.Id, out var c) ? c : [];
            bug.Attachments = attachmentsByEntity.TryGetValue(bug.Id, out var a) 
            ? a.Where(a => a.AttachType == (int)AttachType.Fact || a.AttachType == (int)AttachType.Expected).ToArray() 
            : [];
        }

        foreach (var comment in comments)
        {
            comment.Attachments = attachmentsByEntity.TryGetValue(comment.Id, out var a) 
            ? a.Where(a => a.AttachType == (int)AttachType.Comment).ToArray() 
            : [];
        }

        return report;
    }

    public async Task<bool> GetReportAccessAsync(int reportId, string? organizationId)
    {
        await using var conn = await DataSource.OpenConnectionAsync();
        var report = await conn.QuerySingleOrDefaultAsync<bool>(
            "SELECT * FROM public.get_report_access(@reportId, @organizationId);",
            new { reportId, organizationId }
        );

        return report;
    }

    public async Task<ReportObsoleteDbModel[]> ListReportsAsync(string userId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResults = await connection.QueryAsync<string>(
            "SELECT public.list_reports(@user_id);",
            new { user_id = userId }
        );

        return jsonResults
            .Where(json => json != null)
            .Select(json => Deserialize<ReportObsoleteDbModel>(json)!)
            .ToArray();
    }

    /// <summary>
    /// Создает новый отчет и возвращает его краткую структуру.
    /// </summary>
    public async Task<ReportSummaryDbModel> CreateReportAsync(string userId, string? teamId, string? organizationId, ReportV2CreateDto dto)
    {
        await using var conn = await DataSource.OpenConnectionAsync();
        return await conn.QuerySingleAsync<ReportSummaryDbModel>(
            "SELECT * FROM public.create_report_v2(@userId, @title, @teamId, @organizationId);",
            new { userId, title = dto.Title, teamId, organizationId }
        );
    }

    /// <summary>
    /// Обновляет краткую информацию об отчете и возвращает его краткую структуру.
    /// </summary>
    public async Task<ReportPatchResultDbModel> PatchReportAsync(int reportId, string? organizationId, ReportPatchDto dto)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.QuerySingleAsync<ReportPatchResultDbModel>(
            "SELECT * FROM public.patch_report(@report_id, @organization_id, @title, @status, @responsible_user_id);",
            new
            {
                report_id = reportId,
                organization_id = organizationId,
                title = dto.Title,
                status = dto.Status,
                responsible_user_id = dto.ResponsibleUserId
            }
        );

        return jsonResult;
    }


    /// <summary>
    /// Создает новый отчет и возвращает его полную структуру.
    /// </summary>
    public async Task<ReportObsoleteDbModel?> CreateReportAsync(ReportCreateDbModel reportDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        // Корректная сериализация JSON
        var bugsJson = JsonSerializer.Serialize(reportDbModel.Bugs, JsonSerializerOptions);

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.create_report(@title, @status, @responsible_user_id, @creator_user_id, @participants, @bugs_json);",
            new
            {
                title = reportDbModel.Title,
                status = reportDbModel.Status,
                responsible_user_id = reportDbModel.ResponsibleUserId,
                creator_user_id = reportDbModel.CreatorUserId,
                participants = reportDbModel.ParticipantsUserIds,
                bugs_json = bugsJson
            }
        );

        return jsonResult != null
            ? Deserialize<ReportObsoleteDbModel>(jsonResult)
            : null;
    }

    public async Task<ReportObsoleteDbModel?> UpdateReportAsync(ReportUpdateDbModel reportDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.update_report(@report_id, @participants,@title, @status, @responsible_user_id);",
            new
            {
                report_id = reportDbModel.Id,
                participants = reportDbModel.ParticipantsUserIds,
                title = reportDbModel.Title,
                status = reportDbModel.Status,
                responsible_user_id = reportDbModel.ResponsibleUserId,
            }
        );

        return jsonResult != null
            ? Deserialize<ReportObsoleteDbModel>(jsonResult)
            : null;
    }

    public async Task<SearchReportsDbModel> SearchReportsAsync(SearchReports search)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.search_reports(@sortField, @sortDesc, @skip, @take, @query, @statuses, @userIds);",
            new
            {
                sortField = search.Sort.Field,
                sortDesc = search.Sort.IsDescending,
                skip = (int)search.Skip,
                take = (int)search.Take,
                query = search.Query,
                statuses = search.ReportStatuses,
                userIds = search.UserIds,
            }
        );

        return Deserialize<SearchReportsDbModel>(jsonResult);
    }

    public async Task ChangeStatusAsync(int reportId, int newStatus)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        await connection.ExecuteAsync(
            "SELECT public.change_status_internal(@report_id, @new_status);",
            new { report_id = reportId, new_status = newStatus }
        );
    }

    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
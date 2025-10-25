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
    public async Task<ReportDbModel?> GetReportAsync(int reportId, string? organizationId)
    {
        await using var conn = await DataSource.OpenConnectionAsync();
        // Открываем транзакцию для курсоров
        await using var multi = await conn.QueryMultipleAsync(@"
  SELECT * FROM public.get_report_v2(@reportId, @organizationId);
  SELECT * FROM public.list_bugs_internal(@reportIds);
  SELECT * FROM public.list_participants_internal(@reportIds);
  SELECT * FROM public.list_comments_internal(@reportIds);
  SELECT * FROM public.list_attachments_internal(@reportIds);
", new { reportId, reportIds = new[] { reportId }, organizationId });

        // проверка доступа к репорту
        var report = await multi.ReadSingleOrDefaultAsync<ReportDbModel>();
        if (report == null) return null;

        // 2. Дочерние сущности
        report.Bugs = (await multi.ReadAsync<BugDbModel>()).ToArray();
        var participants = await multi.ReadAsync<(int ReportId, string UserId)>();
        report.ParticipantsUserIds = participants.Select(p => p.UserId).ToArray();
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



    public async Task<(long total, ReportDbModel[] reports)> ListReportsGraphAsync(
    string? organizationId, string? userId, string? teamId, int[]? statuses, int skip, int take)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        var (total, ids) = await ListReportIdsAsync(organizationId, userId, teamId, statuses, skip, take);
        if (ids.Length == 0) return (total, Array.Empty<ReportDbModel>());

        await using var conn = await DataSource.OpenConnectionAsync();

        const string sql = @"
        SELECT * FROM public.list_reports_internal(@ids);

        SELECT * FROM public.list_participants_internal(@ids);

        SELECT * FROM public.list_bugs_internal(@ids);

        SELECT * FROM public.list_comments_internal(@ids);

        SELECT * FROM public.list_attachments_internal(@ids);
    ";

        await using var grid = await conn.QueryMultipleAsync(sql, new { ids });

        var reports = (await grid.ReadAsync<ReportDbModel>()).ToArray();

        var participants = await grid.ReadAsync<(int ReportId, string UserId)>();
        var participantsByReport = participants
            .GroupBy(x => x.ReportId)
            .ToDictionary(g => g.Key, g => g.Select(z => z.UserId).ToArray());

        var bugs = (await grid.ReadAsync<BugDbModel>()).ToArray();
        var bugsByReport = bugs
            .GroupBy(b => b.ReportId)
            .ToDictionary(g => g.Key, g => g.OrderBy(b => b.CreatedAt).ToArray());

        var comments = (await grid.ReadAsync<CommentDbModel>()).ToArray();
        var commentsByBug = comments
            .GroupBy(c => c.BugId)
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.CreatedAt).ToArray());

        var attaches = (await grid.ReadAsync<AttachmentDbModel>()).ToArray();

        // attachments: к багу => AttachType 0/1; к комменту => AttachType 2, entity_id = comment_id
        var bugAttaches = attaches.Where(a => (AttachType)a.AttachType != AttachType.Comment)
                                  .GroupBy(a => a.EntityId)
                                  .ToDictionary(g => g.Key, g => g.OrderBy(a => a.CreatedAt).ToArray());

        var commentAttaches = attaches.Where(a => (AttachType)a.AttachType == AttachType.Comment)
                                      .GroupBy(a => a.EntityId)
                                      .ToDictionary(g => g.Key, g => g.OrderBy(a => a.CreatedAt).ToArray());

        // сборка
        foreach (var r in reports)
        {
            r.ParticipantsUserIds = participantsByReport.GetValueOrDefault(r.Id) ?? Array.Empty<string>();

            var rb = bugsByReport.GetValueOrDefault(r.Id) ?? Array.Empty<BugDbModel>();
            foreach (var b in rb)
            {
                b.Attachments = bugAttaches.GetValueOrDefault(b.Id) ?? Array.Empty<AttachmentDbModel>();
                b.Comments = commentsByBug.GetValueOrDefault(b.Id) ?? Array.Empty<CommentDbModel>();
                foreach (var c in b.Comments)
                    c.Attachments = commentAttaches.GetValueOrDefault(c.Id) ?? Array.Empty<AttachmentDbModel>();
            }
            r.Bugs = rb;
        }

        return (total, reports);
    }

    public async Task<ReportDbModel[]> ListReportsAsync(string? organizationId, string? userId, string? teamId, int[]? reportStatuses, int skip, int take)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResults = await connection.QueryAsync<string>(
            "SELECT public.list_reports_v2(@organization_id, @user_id, @team_id, @report_statuses, @skip, @take);",
            new
            {
                organization_id = organizationId,
                user_id = userId,
                team_id = teamId,
                report_statuses = reportStatuses,
                skip = skip,
                take = take
            }
        );

        return jsonResults
            .Where(json => json != null)
            .Select(json => Deserialize<ReportDbModel>(json)!)
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

    public async Task<SearchReportsDbModel> SearchReportsAsync(SearchReports search)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.search_reports_v2(@sortField, @sortDesc, @skip, @take, @query, @statuses, @userIds, @organizationId);",
            new
            {
                sortField = search.Sort.Field,
                sortDesc = search.Sort.IsDescending,
                skip = (int)search.Skip,
                take = (int)search.Take,
                query = search.Query,
                statuses = search.ReportStatuses,
                userIds = search.UserIds,
                organizationId = search.OrganizationId,
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

    private async Task<(long total, int[] ids)> ListReportIdsAsync(
    string? organizationId, string? userId, string? teamId, int[]? statuses, int skip, int take)
    {
        await using var conn = await DataSource.OpenConnectionAsync();

        const string sql = @"
        SELECT public.list_reports_count(@organizationId, @userId, @teamId, @statuses);
        SELECT id FROM public.list_reports_ids(@skip, @take, @organizationId, @userId, @teamId, @statuses);";

        await using var grid = await conn.QueryMultipleAsync(sql, new
        {
            organizationId,
            userId,
            teamId,
            statuses,
            skip,
            take
        });

        var total = await grid.ReadSingleAsync<long>();
        var ids = (await grid.ReadAsync<int>()).ToArray();
        return (total, ids);
    }

    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
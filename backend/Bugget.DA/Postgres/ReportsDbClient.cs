using System.Text.Json;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Dapper;

namespace Bugget.DA.Postgres;

public sealed class ReportsDbClient: PostgresClient
{
    /// <summary>
    /// Получает отчет по ID.
    /// </summary>
    public async Task<ReportDbModel?> GetReportAsync(int reportId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.get_report(@report_id);",
            new { report_id = reportId }
        );

        return jsonResult != null ? Deserialize<ReportDbModel>(jsonResult) : null;
    }

    /// <summary>
    /// Получает отчет по ID.
    /// </summary>
    public async Task<ReportV2DbModel?> GetReportAsync(int reportId, string? organizationId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.get_report_v2(@report_id, @organization_id);",
            new { report_id = reportId, organization_id = organizationId }
        );

        return jsonResult != null ? Deserialize<ReportV2DbModel>(jsonResult) : null;
    }
    
    public async Task<ReportDbModel[]> ListReportsAsync(string userId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResults = await connection.QueryAsync<string>(
            "SELECT public.list_reports(@user_id);",
            new { user_id = userId }
        );

        return jsonResults
            .Where(json => json != null)
            .Select(json => Deserialize<ReportDbModel>(json)!)
            .ToArray();
    }

    /// <summary>
    /// Создает новый отчет и возвращает его краткую структуру.
    /// </summary>
    public async Task<ReportV2DbModel> CreateReportAsync(string userId, string? teamId, string? organizationId, ReportV2CreateDto dto)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.create_report(@user_id, @title, @team_id, @organization_id);",
            new
            {
                user_id = userId,
                title = dto.Title,
                team_id = teamId,
                organization_id = organizationId,
            }
        );

        return Deserialize<ReportV2DbModel>(jsonResult!)!;
    }

    /// <summary>
    /// Обновляет краткую информацию об отчете и возвращает его краткую структуру.
    /// </summary>
    public async Task<ReportPatchDbModel> PatchReportAsync(int reportId, string userId, string? organizationId, ReportPatchDto dto)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.patch_report(@report_id, @user_id, @organization_id, @title, @status, @responsible_user_id);",
            new
            {
                report_id = reportId,
                user_id = userId,
                organization_id = organizationId,
                title = dto.Title,
                status = dto.Status,
                responsible_user_id = dto.ResponsibleUserId,
            }
        );

        return Deserialize<ReportPatchDbModel>(jsonResult!)!;
    }


    /// <summary>
    /// Создает новый отчет и возвращает его полную структуру.
    /// </summary>
    public async Task<ReportDbModel?> CreateReportAsync(ReportCreateDbModel reportDbModel)
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
            ? Deserialize<ReportDbModel>(jsonResult)
            : null;
    }
    
    public async Task<ReportDbModel?> UpdateReportAsync(ReportUpdateDbModel reportDbModel)
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
            ? Deserialize<ReportDbModel>(jsonResult)
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
    
    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
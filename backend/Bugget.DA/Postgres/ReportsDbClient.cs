using System.Text.Json;
using System.Text.Json.Serialization;
using Bugget.Entities.Config;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Report;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Bugget.DA.Postgres;

public sealed class ReportsDbClient: PostgresClient
{
    /// <summary>
    /// Получает отчет по ID.
    /// </summary>
    public async Task<ReportDbModel?> GetReportAsync(int reportId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.get_report(@report_id);",
            new { report_id = reportId }
        );

        return jsonResult != null ? Deserialize<ReportDbModel>(jsonResult) : null;
    }
    
    public async Task<ReportDbModel[]> ListReportsAsync(string userId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
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
    /// Создает новый отчет и возвращает его полную структуру.
    /// </summary>
    public async Task<ReportDbModel?> CreateReportAsync(ReportCreateDbModel reportDbModel)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);

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
        await using var connection = new NpgsqlConnection(ConnectionString);
        
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
    
    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
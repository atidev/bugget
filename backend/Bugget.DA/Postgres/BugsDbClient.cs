using System.Text.Json;
using System.Text.Json.Serialization;
using Bugget.Entities.Config;
using Bugget.Entities.Constants;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Bug;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Bugget.DA.Postgres;

public sealed class BugsDbClient : PostgresClient
{
    /// <summary>
    /// Создает новый баг и возвращает его полную структуру.
    /// Если репорт не существует, то выбрасывается ошибка 00404, которая обрабатывается на мидлваре.
    /// </summary>
    public async Task<BugDbModel> CreateBugAsync(BugCreateDbModel bugCreateDbModel, string? organizationId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.create_bug(@report_id, @receive, @expect, @creator_user_id, @status, @organization_id);",
            new
            {
                report_id = bugCreateDbModel.ReportId,
                receive = bugCreateDbModel.Receive,
                expect = bugCreateDbModel.Expect,
                creator_user_id = bugCreateDbModel.CreatorUserId,
                status = bugCreateDbModel.Status,
                organization_id = organizationId
            }
        );

        return Deserialize<BugDbModel>(jsonResult!)!;
    }
    
    /// <summary>
    /// Обновляет баг и возвращает его полную структуру.
    /// </summary>
    /// <param name="updateBugDbModel"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public async Task<BugDbModel> UpdateBugObsoleteAsync(BugUpdateDbModel updateBugDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.update_bug(@bug_id, @report_id, @updater_user_id, @receive, @expect, @status);",
            new
            {
                bug_id = updateBugDbModel.Id,
                report_id = updateBugDbModel.ReportId,
                updater_user_id = updateBugDbModel.UpdaterUserId,
                receive = updateBugDbModel.Receive,
                expect = updateBugDbModel.Expect,
                status = updateBugDbModel.Status
            }
        );

        return Deserialize<BugDbModel>(jsonResult!)!;
    }

    /// <summary>
    /// Обновляет баг и возвращает его краткую структуру.
    /// </summary>
    /// <param name="updateBugDbModel"></param>
    /// <param name="organizationId"></param>
    /// <returns></returns>
    public async Task<BugDbModel> UpdateBugSummaryAsync(BugUpdateDbModel updateBugDbModel, string? organizationId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.update_bug_summary(@bug_id, @report_id, @organization_id, @updater_user_id, @receive, @expect, @status);",
            new
            {
                bug_id = updateBugDbModel.Id,
                report_id = updateBugDbModel.ReportId,
                organization_id = organizationId,
                updater_user_id = updateBugDbModel.UpdaterUserId,
                receive = updateBugDbModel.Receive,
                expect = updateBugDbModel.Expect,
                status = updateBugDbModel.Status
            }
        );

        return Deserialize<BugDbModel>(jsonResult!)!;
    }

    public async Task<BugDbModel?> GetBugSummaryAsync(int reportId, int bugId, string? organizationId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.get_bug_summary(@report_id, @bug_id, @organization_id);",
            new { report_id = reportId, bug_id = bugId, organization_id = organizationId }
        );

        return jsonResult is null ? null : Deserialize<BugDbModel>(jsonResult);
    }
    
    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
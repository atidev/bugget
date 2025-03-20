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
    /// Создает новый отчет и возвращает его полную структуру.
    /// </summary>
    public async Task<BugDbModel?> CreateBugAsync(BugCreateDbModel bugCreateDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.create_bug(@report_id, @receive, @expect, @creator_user_id, @status);",
            new
            {
                report_id = bugCreateDbModel.ReportId,
                receive = bugCreateDbModel.Receive,
                expect = bugCreateDbModel.Expect,
                creator_user_id = bugCreateDbModel.CreatorUserId,
                status = bugCreateDbModel.Status,
            }
        );

        return jsonResult != null
            ? Deserialize<BugDbModel>(jsonResult)
            : null;
    }
    
    public async Task<BugDbModel?> UpdateBugAsync(BugUpdateDbModel updateBugDbModel)
    {
        await using var connection = await DataSource.OpenConnectionAsync();
        
        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.update_bug(@bug_id, @report_id,  @updater_user_id, @receive, @expect, @status);",
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

        return jsonResult != null
            ? Deserialize<BugDbModel>(jsonResult)
            : null;
    }

    public async Task<BugDbModel?> GetBug(int bugId)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        var jsonResult = await connection.ExecuteScalarAsync<string>(
            "SELECT public.get_bug(@bugId)",
            new
            {
                bugId
            }
        );

        return jsonResult != null
            ? Deserialize<BugDbModel>(jsonResult)
            : null;
    }
    
    private T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
}
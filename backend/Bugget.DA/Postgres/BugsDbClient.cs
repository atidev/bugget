using System.Text.Json;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO;
using Bugget.Entities.DTO.Bug;
using Dapper;

namespace Bugget.DA.Postgres;

public sealed class BugsDbClient : PostgresClient
{
    public async Task<BugSummaryDbModel> CreateBugAsync(string userId, string? organizationId, int reportId, BugDto bugDto)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleAsync<BugSummaryDbModel>(
            "SELECT public.create_bug_v2(@userId, @organizationId, @reportId, @receive, @expect);",
            new
            {
                userId,
                organizationId,
                reportId,
                receive = bugDto.Receive,
                expect = bugDto.Expect,
            }
                );
    }

    public async Task<BugPatchResultDbModel> PatchBugAsync(int reportId, int bugId, string? organizationId, BugPatchDto patchDto)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleAsync<BugPatchResultDbModel>(
            "SELECT public.patch_bug(@bug_id, @report_id,  @organization_id, @receive, @expect, @status);",
            new
            {
                bug_id = bugId,
                report_id = reportId,
                organization_id = organizationId,
                receive = patchDto.Receive,
                expect = patchDto.Expect,
                status = patchDto.Status
            }
        );
    }

    public async Task<BugDbModel?> CreateBugObsoleteAsync(BugCreateDbModel bugCreateDbModel)
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

    public async Task<BugDbModel?> UpdateBugObsoleteAsync(BugUpdateDbModel updateBugDbModel)
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
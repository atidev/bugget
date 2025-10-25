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
            "SELECT * FROM public.create_bug_v2(@user_id, @organization_id, @report_id, @receive, @expect);",
            new
            {
                user_id = userId,
                organization_id = organizationId,
                report_id = reportId,
                receive = bugDto.Receive,
                expect = bugDto.Expect,
            }
                );
    }

    public async Task<BugPatchResultDbModel> PatchBugAsync(int reportId, int bugId, string? organizationId, BugPatchDto patchDto)
    {
        await using var connection = await DataSource.OpenConnectionAsync();

        return await connection.QuerySingleAsync<BugPatchResultDbModel>(
            "SELECT * FROM public.patch_bug(@bug_id, @report_id,  @organization_id, @receive, @expect, @status);",
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
}
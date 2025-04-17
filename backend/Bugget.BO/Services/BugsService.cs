using Bugget.BO.Errors;
using Bugget.DA.Postgres;
using Bugget.Entities.BO;
using Bugget.Entities.BO.BugBo;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.Mappers;
using Monade;

namespace Bugget.BO.Services;

public sealed class BugsService(BugsDbClient bugsDbClient)
{
    public Task<BugDbModel> CreateBugAsync(Bug bug, string? organizationId)
    {
        return bugsDbClient.CreateBugAsync(bug.ToBugCreateDbModel(), organizationId);
    }

    public Task<BugDbModel> UpdateBugAsync(BugUpdate bug, string? organizationId)
    {
        return bugsDbClient.UpdateBugObsoleteAsync(bug.ToBugUpdateDbModel(), organizationId);
    }

    public Task<BugDbModel> UpdateBugSummaryAsync(BugUpdate bug, string? organizationId)
    {
        return bugsDbClient.UpdateBugSummaryAsync(bug.ToBugUpdateDbModel(), organizationId);
    }

    public async Task<MonadeStruct<BugDbModel>> GetBugSummaryAsync(int reportId, int bugId, string? organizationId)
    {
        var bug = await bugsDbClient.GetBugSummaryAsync(reportId, bugId, organizationId);
        return bug is null ? BoErrors.BugNotFoundError : bug;
    }
}
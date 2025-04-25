using Authentication;
using AutoMapper;
using Bugget.BO.Errors;
using Bugget.BO.Mappers;
using Bugget.DA.Postgres;
using Bugget.Entities.BO;
using Bugget.Entities.BO.BugBo;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO.Bug;
using Monade;
using TaskQueue;
namespace Bugget.BO.Services;

public sealed class BugsService(BugsDbClient bugsDbClient, IMapper mapper, BugsEventsService bugsEventsService, ITaskQueue taskQueue)
{
    public Task<BugDbModel?> CreateBugObsoleteAsync(Bug bug)
    {
        return bugsDbClient.CreateBugObsoleteAsync(bug.ToBugCreateDbModel());
    }

    public Task<BugDbModel?> UpdateBugObsoleteAsync(BugUpdate bug)
    {
        return bugsDbClient.UpdateBugObsoleteAsync(bug.ToBugUpdateDbModel());
    }

    public async Task<MonadeStruct<BugSummaryDbModel>> CreateBugAsync(UserIdentity user, int reportId, BugDto bug)
    {
        if(string.IsNullOrEmpty(bug.Expect) && string.IsNullOrEmpty(bug.Receive))
        {
            return BoErrors.BugMustHaveOneField;
        }
        var bugSummaryDbModel = await bugsDbClient.CreateBugAsync(user.Id, user.OrganizationId, reportId, bug);
        await taskQueue.Enqueue(() => bugsEventsService.HandleCreateBugEventAsync(reportId, user, bugSummaryDbModel));
        return bugSummaryDbModel;
    }

    public async Task<BugPatchResultDbModel> PatchBugAsync(UserIdentity user, int reportId, int bugId, BugPatchDto patchDto)
    {
        var bugPatchResultDbModel = await bugsDbClient.PatchBugAsync(reportId, bugId, user.OrganizationId, patchDto);
        await taskQueue.Enqueue(() => bugsEventsService.HandlePatchBugEventAsync(reportId, bugId, user, patchDto));
        return bugPatchResultDbModel;
    }

    public async Task<Bug> GetBug(int bugId)
    {
        var bugDbModel = await bugsDbClient.GetBug(bugId);
        if (bugDbModel == null)
        {
            throw new Exception("Bug doesn't exist");
        }
        return mapper.Map<Bug>(bugDbModel);
    }
}
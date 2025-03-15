using AutoMapper;
using Bugget.BO.Mappers;
using Bugget.DA.Postgres;
using Bugget.Entities.BO;
using Bugget.Entities.BO.BugBo;
using Bugget.Entities.DbModels.Bug;

namespace Bugget.BO.Services;

public sealed class BugsService(BugsDbClient bugsDbClient, IMapper mapper)
{
    public Task<BugDbModel?> CreateBugAsync(Bug bug)
    {
        return bugsDbClient.CreateBugAsync(bug.ToBugCreateDbModel());
    }

    public Task<BugDbModel?> UpdateBugAsync(BugUpdate bug)
    {
        return bugsDbClient.UpdateBugAsync(bug.ToBugUpdateDbModel());
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
using Bugget.Entities.Adapters;
using Bugget.Entities.BO;
using Bugget.Entities.BO.BugBo;
using Bugget.Entities.Constants;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Attachment;

namespace Bugget.BO.Mappers;

public static class BugMapper
{
    public static BugCreateDbModel ToBugCreateDbModel(this Bug bug)
    {
        return new BugCreateDbModel
        {
            Id = bug.Id,
            ReportId = bug.ReportId,
            Receive = bug.Receive,
            Expect = bug.Expect,
            CreatorUserId = bug.CreatorUserId,
            Status = bug.Status,
        };
    }

    public static BugUpdateDbModel ToBugUpdateDbModel(this BugUpdate bug)
    {
        return new BugUpdateDbModel
        {
            Id = bug.Id,
            ReportId = bug.ReportId,
            Receive = bug.Receive,
            Expect = bug.Expect,
            UpdaterUserId = bug.UpdaterUserId,
            Status = bug.Status,
        };
    }
}
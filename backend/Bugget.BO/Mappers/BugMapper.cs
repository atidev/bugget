using Bugget.Entities.Adapters;
using Bugget.Entities.BO;
using Bugget.Entities.BO.BugBo;
using Bugget.Entities.Constants;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.Views;

namespace Bugget.BO.Mappers;

public static class BugMapper
{
    public static Bug ToBug(this BugDto bug, int reportId, string creatorUserId)
    {
        return new Bug
        {
            ReportId = reportId,
            Receive = bug.Receive,
            Expect = bug.Expect,
            CreatorUserId = creatorUserId
        };
    }

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

    public static BugViewObsolete ToViewObsolete(this BugDbModel bug, IReadOnlyDictionary<string, EmployeeObsolete> employeesDict)
    {
        return new BugViewObsolete
        {
            Id = bug.Id,
            ReportId = bug.ReportId,
            Receive = bug.Receive,
            Expect = bug.Expect,
            Creator = employeesDict.TryGetValue(bug.CreatorUserId, out var e)
                ? EmployeeAdapter.ToUserView(e)
                : EmployeeAdapter.ToUserView(bug.CreatorUserId),
            CreatedAt = bug.CreatedAt,
            UpdatedAt = bug.UpdatedAt,
            Status = bug.Status,
            Attachments = bug.Attachments?.Select(a => new AttachmentView
            {
                Id = a.Id,
                BugId = bug.Id,
                ReportId = bug.ReportId,
                Path = a.Path,
                CreatedAt = a.CreatedAt!.Value,
                AttachType = a.AttachType
            }).ToArray(),
            Comments = bug.Comments?.Select(c => c.ToCommentViewObsolete(employeesDict))
                .ToArray(),
        };
    }

    public static BugView ToView(this BugDbModel bug)
    {
        return new BugView
        {
            Id = bug.Id,
            ReportId = bug.ReportId,
            Receive = bug.Receive,
            Expect = bug.Expect,
            CreatorUserId = bug.CreatorUserId,
            CreatedAt = bug.CreatedAt,
            UpdatedAt = bug.UpdatedAt,
            Status = bug.Status,
            Attachments = bug.Attachments?.Select(a => new AttachmentView
            {
                Id = a.Id,
                BugId = bug.Id,
                ReportId = bug.ReportId,
                Path = a.Path,
                CreatedAt = a.CreatedAt!.Value,
                AttachType = a.AttachType
            }).ToArray(),
            Comments = bug.Comments?.Select(c => c.ToCommentView())
                .ToArray(),
        };
    }

    public static BugUpdate ToBugUpdate(this BugUpdateDto bugUpdateDto, int reportId, int bugId, string userId)
    {
        return new BugUpdate
        {
            Id = bugId,
            ReportId = reportId,
            UpdaterUserId = userId,
            Status = bugUpdateDto.Status,
            Receive = bugUpdateDto.Receive,
            Expect = bugUpdateDto.Expect,
        };
    }
}
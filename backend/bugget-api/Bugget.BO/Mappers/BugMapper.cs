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
    public static Bug ToBug(this BugDtoObsolete bug, int reportId, string creatorUserId)
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

    public static BugView ToView(this BugDbModel bug, IReadOnlyDictionary<string, User> usersDict)
    {
        return new BugView
        {
            Id = bug.Id,
            ReportId = bug.ReportId,
            Receive = bug.Receive,
            Expect = bug.Expect,
            Creator = usersDict.TryGetValue(bug.CreatorUserId, out var e)
                ? UsersAdapter.ToUserView(e)
                : UsersAdapter.ToUserView(bug.CreatorUserId),
            CreatedAt = bug.CreatedAt,
            UpdatedAt = bug.UpdatedAt,
            Status = bug.Status,
            Attachments = bug.Attachments?.Select(a => new AttachmentView
            {
                Id = a.Id,
                BugId = bug.Id,
                ReportId = bug.ReportId,
                EntityId = a.EntityId!.Value,
                AttachType = a.AttachType,
                CreatedAt = a.CreatedAt,
                HasPreview = a.HasPreview.Value,
                FileName = a.FileName ?? string.Empty,
                CreatorUserId = a.CreatorUserId ?? string.Empty,
            }).ToArray(),
            Comments = bug.Comments?.Select(c => c.ToCommentView(bug.ReportId, usersDict))
                .ToArray(),
        };
    }

    public static BugUpdate ToBugUpdate(this BugUpdateDtoObsolete bugUpdateDto, int reportId, int bugId, string userId)
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
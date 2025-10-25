using Bugget.Entities.Adapters;
using Bugget.Entities.BO;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.SocketViews;
using Bugget.Entities.Views;

namespace Bugget.BO.Mappers;

public static class ReportMapper
{
    public static ReportView ToView(this ReportDbModel report, IReadOnlyDictionary<string, User> usersDict)
    {
        return new ReportView
        {
            Id = report.Id,
            Title = report.Title,
            Status = report.Status,
            Responsible = usersDict.TryGetValue(report.ResponsibleUserId, out var er)
                ? UsersAdapter.ToUserView(er)
                : UsersAdapter.ToUserView(report.ResponsibleUserId),
            Creator = usersDict.TryGetValue(report.CreatorUserId, out var ec)
                ? UsersAdapter.ToUserView(ec)
                : UsersAdapter.ToUserView(report.CreatorUserId),
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt,
            Participants = report.ParticipantsUserIds.Select(p =>
                usersDict.TryGetValue(p, out var e)
                    ? UsersAdapter.ToUserView(e)
                    : UsersAdapter.ToUserView(p)).ToArray(),
            Bugs = report.Bugs?.Select(b => b.ToView(usersDict)).ToArray()
        };
    }

    public static SearchReportsView ToView(this SearchReportsDbModel search, IReadOnlyDictionary<string, User> usersDict)
    {
        return new SearchReportsView
        {
            Reports = search.Reports.Select(r => ToView(r, usersDict)).ToArray(),
            Total = search.Total
        };
    }

    public static Report ToReport(this ReportCreateDto report, string userId)
    {
        return new Report
        {
            Title = report.Title,
            ResponsibleUserId = report.ResponsibleId,
            CreatorUserId = userId,
            Bugs = report.Bugs.Select(b => new Bug
            {
                Receive = b.Receive,
                Expect = b.Expect,
                CreatorUserId = userId,
            })
                .ToArray(),
            ParticipantsUserIds = new string[] { userId, report.ResponsibleId }.Distinct().ToArray()
        };
    }

    public static ReportUpdate ToReportUpdate(this ReportPatchDto report, int reportId, string userId)
    {
        return new ReportUpdate
        {
            Id = reportId,
            Title = report.Title,
            UpdaterUserId = userId,
            Status = report.Status,
            ResponsibleUserId = report.ResponsibleUserId
        };
    }

    public static ReportCreateDbModel ToReportDbModel(this Report report)
    {
        return new ReportCreateDbModel
        {
            Title = report.Title,
            Status = report.Status,
            ResponsibleUserId = report.ResponsibleUserId,
            CreatorUserId = report.CreatorUserId,
            ParticipantsUserIds = report.ParticipantsUserIds,
            Bugs = report.Bugs.Select(b => new BugCreateDbModel
            {
                Receive = b.Receive,
                Expect = b.Expect,
                CreatorUserId = b.CreatorUserId,
                Status = b.Status
            }).ToArray()
        };
    }

    public static ReportUpdateDbModel ToReportUpdateDbModel(this ReportUpdate report)
    {
        return new ReportUpdateDbModel
        {
            Id = report.Id,
            Title = report.Title,
            Status = report.Status,
            ResponsibleUserId = report.ResponsibleUserId,
            ParticipantsUserIds = report.ParticipantsUserIds,
        };
    }

    public static SearchReports ToSearchReports(
        string? query,
        int[]? reportStatuses,
        string? userId,
        string? teamId,
        string? organizationId,
        string? sort,
        uint skip,
        uint take,
        IReadOnlyDictionary<string, IReadOnlyCollection<User>> usersByTeam)
    {
        List<string> resultUserIds = [];
        if (!string.IsNullOrEmpty(teamId))
        {
            if (usersByTeam.TryGetValue(teamId, out var team))
            {
                resultUserIds.AddRange(team.Select(e => e.Id));
            }
        }

        if (!string.IsNullOrEmpty(userId))
        {
            resultUserIds.AddRange(userId);
        }

        return new SearchReports
        {
            Query = string.IsNullOrEmpty(query) ? null : query,
            ReportStatuses = reportStatuses?.Length > 0 ? reportStatuses : null,
            UserIds = resultUserIds.Count > 0 ? resultUserIds.ToArray() : null,
            Skip = skip,
            Take = take,
            Sort = SortOption.Parse(sort),
            OrganizationId = organizationId
        };
    }

    public static PatchReportSocketView ToSocketView(this ReportPatchDto patchDto, ReportPatchResultDbModel result)
    {
        return new PatchReportSocketView
        {
            Title = patchDto.Title,
            Status = patchDto.Status,
            ResponsibleUserId = patchDto.ResponsibleUserId,
            PastResponsibleUserId = patchDto.ResponsibleUserId == null ? null : result.PastResponsibleUserId,
            UpdatedAt = result.UpdatedAt
        };
    }
}
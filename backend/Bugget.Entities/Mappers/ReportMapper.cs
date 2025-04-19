using Bugget.Entities.BO;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Summary;

namespace Bugget.Entities.Mappers;

public static class ReportMapper
{
    public static ReportViewObsolete ToViewObsolete(this ReportDbModel report, IReadOnlyDictionary<string, EmployeeObsolete> employeesDict)
    {
        return new ReportViewObsolete
        {
            Id = report.Id,
            Title = report.Title,
            Status = report.Status,
            Responsible = employeesDict.TryGetValue(report.ResponsibleUserId, out var er)
                ? EmployeesMapper.ToUserView(er)
                : EmployeesMapper.ToUserView(report.ResponsibleUserId),
            Creator = employeesDict.TryGetValue(report.CreatorUserId, out var ec)
                ? EmployeesMapper.ToUserView(ec)
                : EmployeesMapper.ToUserView(report.CreatorUserId),
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt,
            Participants = report.ParticipantsUserIds.Select(p =>
                employeesDict.TryGetValue(p, out var e)
                    ? EmployeesMapper.ToUserView(e)
                    : EmployeesMapper.ToUserView(p)).ToArray(),
            Bugs = report.Bugs.Select(b => b.ToViewObsolete(employeesDict)).ToArray()
        };
    }

    public static ReportView ToView(this ReportDbModel report)
    {
        return new ReportView
        {
            Id = report.Id,
            Title = report.Title,
            Status = report.Status,
            ResponsibleUserId = report.ResponsibleUserId,
            CreatorUserId = report.CreatorUserId,
            CreatorTeamId = report.CreatorTeamId,
            CreatorOrganizationId = report.CreatorOrganizationId,
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt,
            ParticipantsUserIds = report.ParticipantsUserIds,
            Bugs = report.Bugs!.Select(b => b.ToView()).ToArray()
        };
    }

    public static SearchReportsView<ReportViewObsolete> ToViewObsolete(this SearchReportsDbModel search, IReadOnlyDictionary<string, EmployeeObsolete> employeesDict)
    {
        return new SearchReportsView<ReportViewObsolete>
        {
            Reports = search.Reports.Select(r => ToViewObsolete(r, employeesDict)).ToArray(),
            Total = search.Total
        };
    }

    public static SearchReportsView<ReportView> ToView(this SearchReportsDbModel search)
    {
        return new SearchReportsView<ReportView>
        {
            Reports = search.Reports.Select(r => ToView(r)).ToArray(),
            Total = search.Total
        };
    }

    public static Report ToReport(this ReportCreateDto report, string userId, string? organizationId, string? teamId)
    {
        return new Report
        {
            Title = report.Title,
            ResponsibleUserId = report.ResponsibleId,
            CreatorUserId = userId,
            CreatorOrganizationId = organizationId,
            CreatorTeamId = teamId,
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

    public static ReportUpdate ToReportUpdate(this ReportUpdateDto report, int reportId, string userId)
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
            CreatorTeamId = report.CreatorTeamId,
            CreatorOrganizationId = report.CreatorOrganizationId,
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

    public static SearchReportsObsolete ToSearchReportsObsolete(
        string? query,
        int[]? reportStatuses,
        string? userId,
        string? teamId,
        string? sort,
        uint skip,
        uint take,
        IReadOnlyDictionary<string, IReadOnlyCollection<EmployeeObsolete>> employeesByTeam)
    {
        List<string> resultUserIds = [];
        if (!string.IsNullOrEmpty(teamId))
        {
            if (employeesByTeam.TryGetValue(teamId, out var team))
            {
                resultUserIds.AddRange(team.Select(e => e.Id));
            }
        }

        if (!string.IsNullOrEmpty(userId))
        {
            resultUserIds.AddRange(userId);
        }

        return new SearchReportsObsolete
        {
            Query = string.IsNullOrEmpty(query) ? null : query,
            ReportStatuses = reportStatuses?.Length > 0 ? reportStatuses : null,
            UserIds = resultUserIds.Count > 0 ? resultUserIds.ToArray() : null,
            Skip = skip,
            Take = take,
            Sort = SortOption.Parse(sort)
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
        uint take)
    {
        return new SearchReports
        {
            Query = string.IsNullOrWhiteSpace(query) ? null : query,
            ReportStatuses = reportStatuses?.Length > 0 ? reportStatuses : null,
            UserId = userId,
            TeamId = teamId,
            OrganizationId = organizationId,
            Skip = skip,
            Take = take,
            Sort = SortOption.Parse(sort)
        };
    }

    public static ReportSummaryView ToSummaryView(this ReportDbModel report)
    {
        return new ReportSummaryView
        {
            Id = report.Id,
            Title = report.Title,
            Status = report.Status,
            ResponsibleUserId = report.ResponsibleUserId,
            UpdatedAt = report.UpdatedAt,
            CreatorUserId = report.CreatorUserId,
            CreatedAt = report.CreatedAt
        };
    }
}
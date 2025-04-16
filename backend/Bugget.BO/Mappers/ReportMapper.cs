using Bugget.Entities.Adapters;
using Bugget.Entities.BO;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.Views;

namespace Bugget.BO.Mappers;

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
                ? EmployeeAdapter.ToUserView(er)
                : EmployeeAdapter.ToUserView(report.ResponsibleUserId),
            Creator = employeesDict.TryGetValue(report.CreatorUserId, out var ec)
                ? EmployeeAdapter.ToUserView(ec)
                : EmployeeAdapter.ToUserView(report.CreatorUserId),
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt,
            Participants = report.ParticipantsUserIds.Select(p =>
                employeesDict.TryGetValue(p, out var e)
                    ? EmployeeAdapter.ToUserView(e)
                    : EmployeeAdapter.ToUserView(p)).ToArray(),
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
            CreatedAt = report.CreatedAt,
            UpdatedAt = report.UpdatedAt,
            ParticipantsUserIds = report.ParticipantsUserIds,
            Bugs = report.Bugs.Select(b => b.ToView()).ToArray()
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

    public static Report ToReport(this ReportCreateDto report, string userId)
    {
        return new Report
        {
            Title = report.Title,
            CreatorUserId = userId,
            ResponsibleUserId = report.ResponsibleUserId,
            ParticipantsUserIds = report.ParticipantsUserIds,
            Status = report.Status
        };
    }

    public static ReportUpdate ToReportUpdate(this ReportUpdateDto report, int reportId, string userId)
    {
        return new ReportUpdate
        {
            Id = reportId,
            Title = report.Title,
            UpdaterUserId = userId,
            ResponsibleUserId = report.ResponsibleUserId,
            ParticipantsUserIds = report.ParticipantsUserIds,
            Status = report.Status
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
            Skip = skip,
            Take = take,
            Sort = SortOption.Parse(sort)
        };
    }
}
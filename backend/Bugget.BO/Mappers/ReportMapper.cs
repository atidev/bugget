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
    public static ReportView ToView(this ReportDbModel report, IReadOnlyDictionary<string, Employee> employeesDict)
    {
        return new ReportView
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
            Bugs = report.Bugs.Select(b => b.ToView(employeesDict)).ToArray()
        };
    }

    public static SearchReportsView ToView(this SearchReportsDbModel search, IReadOnlyDictionary<string, Employee> employeesDict)
    {
        return new SearchReportsView
        {
            Reports = search.Reports.Select(r => ToView(r, employeesDict)).ToArray(),
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
        string[]? userIds,
        string[]? teamIds,
        string? sort,
        uint skip,
        uint take,
        IReadOnlyDictionary<string, IReadOnlyCollection<Employee>> employeesByTeam)
    {
        List<string> resultUserIds = [];
        if (teamIds?.Length >= 0)
        {
            foreach (var teamId in teamIds)
            {
                if (employeesByTeam.TryGetValue(teamId, out var team))
                {
                    resultUserIds.AddRange(team.Select(e => e.Id));
                }
            }
        }

        if (userIds?.Length >= 0)
        {
            resultUserIds.AddRange(userIds);
        }

        return new SearchReports
        {
            Query = string.IsNullOrEmpty(query) ? null : query,
            ReportStatuses = reportStatuses?.Length > 0 ? reportStatuses : null,
            UserIds = resultUserIds.Count > 0 ? resultUserIds.ToArray() : null,
            Skip = skip,
            Take = take,
            Sort = SortOption.Parse(sort)
        };
    }
}
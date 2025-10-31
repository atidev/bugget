using Bugget.Entities.Authentication;
using Bugget.BO.Errors;
using Bugget.BO.Mappers;
using Bugget.DA.Postgres;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.ExternalClients;
using Bugget.ExternalClients.Context;
using Microsoft.Extensions.Logging;
using Monade;
using TaskQueue;

namespace Bugget.BO.Services.Reports;

public sealed class ReportsService(
    ReportsDbClient reportsDbClient,
    ExternalClientsActionService externalClientsActionService,
    ITaskQueue taskQueue,
    ReportEventsService reportEventsService)
{

    public Task<ReportSummaryDbModel> CreateReportAsync(string userId, string? teamId, string? organizationId, ReportCreateDto createDto)
    {
        return reportsDbClient.CreateReportAsync(userId, teamId, organizationId, createDto);
    }

    public async Task<ReportPatchResultDbModel> PatchReportAsync(int reportId, UserIdentity user, ReportPatchDto patchDto)
    {
        var result = await reportsDbClient.PatchReportAsync(reportId, user.OrganizationId, patchDto);

        await taskQueue.EnqueueAsync(() => reportEventsService.HandlePatchReportEventAsync(reportId, user, patchDto, result));

        return result;
    }

    public async Task<MonadeStruct<ReportDbModel>> GetReportAsync(int reportId, string? organizationId)
    {
        var report = await reportsDbClient.GetReportAsync(reportId, organizationId);
        if (report == null)
        {
            return BoErrors.ReportNotFoundError;
        }

        return ApplyBoSort(report);
    }

    public async Task<bool> GetReportAccessAsync(int reportId, string? organizationId)
    {
        var reportAccess = await reportsDbClient.GetReportAccessAsync(reportId, organizationId);
        if (reportAccess)
        {
            return true;
        }

        return false;
    }

    public Task<(long Total, ReportDbModel[] Reports)> ListReportsAsync(string? organizationId, string? userId, string? teamId, int[]? reportStatuses, int skip, int take)
    {
        return reportsDbClient.ListReportsAsync(organizationId, userId, teamId, reportStatuses, skip, take);
    }

    public Task<(long Total, ReportDbModel[] Reports)> SearchReportsAsync(SearchReports search)
    {
        return reportsDbClient.SearchReportsAsync(search);
    }

    private static ReportDbModel ApplyBoSort(ReportDbModel report)
    {
        report.Bugs = report.Bugs?.OrderBy(b => b.Status).ThenBy(b => b.CreatedAt).ToArray();
        return report;
    }
}
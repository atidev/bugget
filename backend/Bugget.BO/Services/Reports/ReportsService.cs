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
    ReportEventsService reportEventsService,
    ILogger<ReportsService> logger)
{

    public Task<ReportSummaryDbModel> CreateReportAsync(string userId, string? teamId, string? organizationId, ReportV2CreateDto createDto)
    {
        return reportsDbClient.CreateReportAsync(userId, teamId, organizationId, createDto);
    }

    public async Task<ReportPatchResultDbModel> PatchReportAsync(int reportId, UserIdentity user, ReportPatchDto patchDto)
    {
        logger.LogInformation("Пользователь {@UserId} патчит отчёт {@ReportId}, {@PatchDto}", user.Id, reportId, patchDto);

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

        return report;
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

    public async Task<ReportObsoleteDbModel?> CreateReportObsoleteAsync(Report report)
    {
        var reportDbModel = await reportsDbClient.CreateReportAsync(report.ToReportDbModel());
        if (reportDbModel == null)
        {
            return null;
        }

        await taskQueue.EnqueueAsync(() => externalClientsActionService.ExecuteReportCreatePostActions(new ReportCreateContext(report, reportDbModel)));

        return reportDbModel;
    }

    public Task<ReportObsoleteDbModel[]> ListReportsAsync(string userId)
    {
        return reportsDbClient.ListReportsAsync(userId);
    }

    public Task<ReportObsoleteDbModel?> GetReportObsoleteAsync(int reportId)
    {
        return reportsDbClient.GetReportAsync(reportId);
    }

    public async Task<ReportObsoleteDbModel?> UpdateReportObsoleteAsync(ReportUpdate report)
    {
        var reportDbModel = await reportsDbClient.UpdateReportAsync(report.ToReportUpdateDbModel());

        if (reportDbModel == null)
            return null;

        await taskQueue.EnqueueAsync(() => externalClientsActionService.ExecuteReportUpdatePostActions(new ReportUpdateContext(report, reportDbModel)));

        return reportDbModel;
    }

    public Task<SearchReportsDbModel> SearchReportsAsync(SearchReports search)
    {
        return reportsDbClient.SearchReportsAsync(search);
    }
}
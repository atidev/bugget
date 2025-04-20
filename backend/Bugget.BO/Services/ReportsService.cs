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

namespace Bugget.BO.Services;

public sealed class ReportsService(
    ReportsDbClient reportsDbClient,
    ExternalClientsActionService externalClientsActionService,
    ITaskQueue taskQueue,
    ReportEventsService reportEventsService,
    ILogger<ReportsService> logger)
{
    public async Task<ReportObsoleteDbModel?> CreateReportAsync(Report report)
    {
        var reportDbModel = await reportsDbClient.CreateReportAsync(report.ToReportDbModel());
        if (reportDbModel == null)
        {
            return null;
        }

        await taskQueue.Enqueue(() => externalClientsActionService.ExecuteReportCreatePostActions(new ReportCreateContext(report, reportDbModel)));

        return reportDbModel;
    }

    public Task<ReportSummaryDbModel> CreateReportAsync(string userId, string? teamId, string? organizationId, ReportV2CreateDto createDto)
    {
        return reportsDbClient.CreateReportAsync(userId, teamId, organizationId, createDto);
    }

    public async Task<ReportPatchResultDbModel> PatchReportAsync(int reportId, string userId, string? organizationId, ReportPatchDto patchDto)
    {
        logger.LogInformation("Пользователь {@UserId} патчит отчёт {@ReportId}, {@PatchDto}", userId, reportId, patchDto);

        var result = await reportsDbClient.PatchReportAsync(reportId, userId, organizationId, patchDto);

        await taskQueue.Enqueue(() => reportEventsService.HandlePatchReportEventAsync(reportId, userId, patchDto, result));

        return result;
    }

    public Task<ReportObsoleteDbModel[]> ListReportsAsync(string userId)
    {
        return reportsDbClient.ListReportsAsync(userId);
    }

    public Task<ReportObsoleteDbModel?> GetReportAsync(int reportId)
    {
        return reportsDbClient.GetReportAsync(reportId);
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

    public async Task<ReportObsoleteDbModel?> UpdateReportAsync(ReportUpdate report)
    {
        var reportDbModel = await reportsDbClient.UpdateReportAsync(report.ToReportUpdateDbModel());

        if (reportDbModel == null)
            return null;

        await taskQueue.Enqueue(() => externalClientsActionService.ExecuteReportUpdatePostActions(new ReportUpdateContext(report, reportDbModel)));

        return reportDbModel;
    }

    public Task<SearchReportsDbModel> SearchReportsAsync(SearchReports search)
    {
        return reportsDbClient.SearchReportsAsync(search);
    }
}
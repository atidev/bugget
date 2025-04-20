using Bugget.BO.Errors;
using Bugget.BO.Mappers;
using Bugget.BO.WebSockets;
using Bugget.DA.Postgres;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.SocketViews;
using Bugget.ExternalClients;
using Bugget.ExternalClients.Context;
using Monade;
using TaskQueue;

namespace Bugget.BO.Services;

public sealed class ReportsService(
    ReportsDbClient reportsDbClient,
    ExternalClientsActionService externalClientsActionService,
    ITaskQueue taskQueue,
    IReportPageHubClient reportPageHubClient)
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
        var result = await reportsDbClient.PatchReportAsync(reportId, userId, organizationId, patchDto);
        var socketView = new PatchReportSocketView
        {
            Title = patchDto.Title,
            Status = patchDto.Status,
            ResponsibleUserId = patchDto.ResponsibleUserId,
            PastResponsibleUserId = patchDto.ResponsibleUserId == null ? null : result.PastResponsibleUserId
        };

        await reportPageHubClient.SendReportPatchAsync(reportId, socketView);

        await taskQueue.Enqueue(() => externalClientsActionService.ExecuteReportPatchPostActions(new ReportPatchContext(userId, patchDto, result)));

        // todo проставление участников и уведомление
        
        // todo вычисление статуса
        
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
using Bugget.DA.Postgres;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels.Report;
using Bugget.Features;
using Bugget.Features.Context;
using Bugget.BO.Errors;
using Bugget.Entities.Mappers;
using Monade;

namespace Bugget.BO.Services;

public sealed class ReportsService(
    ReportsDbClient reportsDbClient,
    FeaturesService featuresService)
{
    public async Task<ReportDbModel> CreateReportAsync(Report report)
    {
        var reportDbModel = await reportsDbClient.CreateReportAsync(report.ToReportDbModel());

        await featuresService.ExecuteReportCreatePostActions(new ReportCreateContext(report, reportDbModel));

        return reportDbModel;
    }

    public Task<ReportDbModel[]> ListReportsAsync(string userId)
    {
        return reportsDbClient.ListReportsAsync(userId);
    }

    public async Task<MonadeStruct<ReportDbModel>> GetReportAsync(int reportId, string? organizationId)
    {
        var report = await reportsDbClient.GetReportAsync(reportId, organizationId);
        if (report == null)
        {
            return new MonadeStruct<ReportDbModel>(BoErrors.ReportNotFoundError);
        }

        return report;
    }

    public async Task<ReportDbModel?> UpdateReportObsoleteAsync(ReportUpdate report)
    {
        var reportDbModel =  await reportsDbClient.UpdateReportAsync(report.ToReportUpdateDbModel());

        if (reportDbModel == null)
            return null;
        
        await featuresService.ExecuteReportUpdatePostActions(new ReportUpdateContext(report, reportDbModel));

        return reportDbModel;
    }

    public async Task<ReportDbModel> UpdateReportSummaryAsync(ReportUpdate report, string? organizationId)
    {
        var reportDbModel =  await reportsDbClient.UpdateReportSummaryAsync(report.ToReportUpdateDbModel(), organizationId);
        
        await featuresService.ExecuteReportUpdatePostActions(new ReportUpdateContext(report, reportDbModel));

        return reportDbModel;
    }
    
    public Task<SearchReportsDbModel> SearchReportsAsync(SearchReportsObsolete search)
    {
        return reportsDbClient.SearchReportsObsoleteAsync(search);
    }

    public Task<SearchReportsDbModel> SearchReportsAsync(SearchReports search)
    {
        return reportsDbClient.SearchReportsAsync(search);
    }

    public async Task<MonadeStruct<ReportDbModel>> GetReportSummaryAsync(int reportId, string? organizationId)
    {
        var report = await reportsDbClient.GetReportSummaryAsync(reportId, organizationId);
        if (report == null)
        {
            return new MonadeStruct<ReportDbModel>(BoErrors.ReportNotFoundError);
        }

        return report;
    }
}
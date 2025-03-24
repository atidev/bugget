using Bugget.BO.Mappers;
using Bugget.DA.Postgres;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.BO.Search;
using Bugget.Entities.DbModels.Report;
using Bugget.Features;
using Bugget.Features.Context;

namespace Bugget.BO.Services;

public sealed class ReportsService(
    ReportsDbClient reportsDbClient,
    FeaturesService featuresService)
{
    public async Task<ReportDbModel?> CreateReportAsync(Report report)
    {
        var reportDbModel = await reportsDbClient.CreateReportAsync(report.ToReportDbModel());
        if (reportDbModel == null)
        {
            return null;
        }

        await featuresService.ExecuteReportCreatePostActions(new ReportCreateContext(report, reportDbModel));

        return reportDbModel;
    }

    public Task<ReportDbModel[]> ListReportsAsync(string userId)
    {
        return reportsDbClient.ListReportsAsync(userId);
    }

    public Task<ReportDbModel?> GetReportAsync(int reportId)
    {
        return reportsDbClient.GetReportAsync(reportId);
    }

    public async Task<ReportDbModel?> UpdateReportAsync(ReportUpdate report)
    {
        var reportDbModel =  await reportsDbClient.UpdateReportAsync(report.ToReportUpdateDbModel());

        if (reportDbModel == null)
            return null;
        
        await featuresService.ExecuteReportUpdatePostActions(new ReportUpdateContext(report, reportDbModel));

        return reportDbModel;
    }
    
    public Task<SearchReportsDbModel> SearchReportsAsync(SearchReports search)
    {
        return reportsDbClient.SearchReportsAsync(search);
    }
}
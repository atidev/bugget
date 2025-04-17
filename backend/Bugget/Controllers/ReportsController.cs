using Bugget.Authentication;
using Bugget.BO.Mappers;
using Bugget.BO.Services;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Summary;
using Bugget.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с репортами
/// </summary>
[LdapAuth]
[Route("bugget/public/v2/reports")]
public sealed class ReportsController(
    ReportsService reportsService,
    IHubContext<ReportPageHub> hubContext) : ApiController
{
    /// <summary>
    /// Создать репорт
    /// </summary>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(ReportView), 200)]
    public async Task<ReportView?> CreateReportAsync([FromBody] ReportCreateDto createDto)
    {
        var user = User.GetIdentity();
        var createdReport = await reportsService.CreateReportAsync(createDto.ToReport(user.Id, user.TeamId, user.OrganizationId));
        return createdReport?.ToView();
    }

    /// <summary>
    /// Получить репорты (для главной страницы)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ReportView[]), 200)]
    public async Task<ReportView[]> ListReportsAsync()
    {
        var user = User.GetIdentity();
        var reports = await reportsService.ListReportsAsync(user.Id);
        return reports.Select(r => r.ToView()).ToArray();
    }

    /// <summary>
    /// Получить репорт
    /// </summary>
    /// <param name="reportId"></param>
    /// <returns></returns>
    [HttpGet("{reportId}")]
    [ProducesResponseType(typeof(ReportDbModel), 200)]
    public async Task<ReportView?> GetReportAsync([FromRoute] int reportId)
    {
        var report = await reportsService.GetReportAsync(reportId);
        return report?.ToView();
    }

    /// <summary>
    /// Изменить репорт
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{reportId}/summary")]
    [ProducesResponseType(typeof(ReportSummaryView), 200)]
    public async Task<ReportSummaryView?> UpdateReportSummaryAsync([FromRoute] int reportId, [FromBody] ReportUpdateDto updateDto)
    {
        var user = User.GetIdentity();
        var report = await reportsService.UpdateReportSummaryAsync(updateDto.ToReportUpdate(reportId, user.Id), user.OrganizationId);

        await hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReport");

        return report?.ToSummaryView();
    }

    /// <summary>
    /// Поиск по репортам
    /// </summary>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchReportsView<ReportView>), 200)]
    public async Task<SearchReportsView<ReportView>> SearchReportsAsync(
        [FromQuery] string? query,
        [FromQuery] int[]? reportStatuses,
        [FromQuery] string? userId,
        [FromQuery] string? teamId,
        [FromQuery] string? sort,
        [FromQuery] uint skip = 0,
        [FromQuery] uint take = 10
        )
    {
        var user = User.GetIdentity();
        var searchResult = await reportsService.SearchReportsAsync(
            ReportMapper.ToSearchReports(
                query,
                reportStatuses,
                userId,
                teamId,
                user.OrganizationId,
                sort,
                skip,
                take
                ));
        
        return searchResult.ToView();
    }

    /// <summary>
    /// Получить краткую информацию о репорте
    /// </summary>
    /// <param name="reportId"></param>
    /// <returns></returns>
    [HttpGet("{reportId}/summary")]
    [ProducesResponseType(typeof(ReportSummaryView), 200)]
    public async Task<ReportSummaryView?> GetReportSummaryAsync([FromRoute] int reportId)
    {
        var user = User.GetIdentity();
        var report = await reportsService.GetReportSummaryAsync(reportId, user.OrganizationId);
        return report?.ToSummaryView();
    }
} 
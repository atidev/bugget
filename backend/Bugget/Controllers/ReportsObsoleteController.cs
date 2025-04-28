using Authentication;
using Bugget.BO.Mappers;
using Bugget.BO.Services;
using Bugget.DA.Files;
using Bugget.Entities.BO;
using Bugget.Entities.BO.ReportBo;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.Views;
using Bugget.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с репортами
/// </summary>
[Auth]
[Route("bugget/public/v1/reports")]
[Obsolete("Используйте ReportsV2Controller")]
public sealed class ReportsObsoleteController(
    ReportsService reportsService,
    IHubContext<ReportPageHub> hubContext,
    EmployeesDataAccess employeesDataAccess) : ApiController
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
        var createdReport = await reportsService.CreateReportObsoleteAsync(createDto.ToReport(user.Id));

        return createdReport?.ToView(employeesDataAccess.DictEmployees());
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
        return reports.Select(r => r.ToView(employeesDataAccess.DictEmployees())).ToArray();
    }

    /// <summary>
    /// Получить репорт
    /// </summary>
    /// <param name="reportId"></param>
    /// <returns></returns>
    [HttpGet("{reportId}")]
    [ProducesResponseType(typeof(ReportObsoleteDbModel), 200)]
    public async Task<ReportView?> GetReportAsync([FromRoute] int reportId)
    {
        var report = await reportsService.GetReportObsoleteAsync(reportId);
        return report?.ToView(employeesDataAccess.DictEmployees());
    }

    /// <summary>
    /// Изменить репорт
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{reportId}")]
    [ProducesResponseType(typeof(ReportView), 200)]
    public async Task<ReportView?> UpdateReportAsync([FromRoute] int reportId, [FromBody] ReportPatchDto updateDto)
    {
        var user = User.GetIdentity();
        var report = await reportsService.UpdateReportObsoleteAsync(updateDto.ToReportUpdate(reportId, user.Id));

        await hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReport");

        return report?.ToView(employeesDataAccess.DictEmployees());
    }

    /// <summary>
    /// Поиск по репортам
    /// </summary>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchReportsView), 200)]
    public async Task<SearchReportsView> SearchReportsAsync(
        [FromQuery] string? query,
        [FromQuery] int[]? reportStatuses,
        [FromQuery] string? userId,
        [FromQuery] string? teamId,
        [FromQuery] string? sort,
        [FromQuery] uint skip = 0,
        [FromQuery] uint take = 10
        )
    {
        var searchResult = await reportsService.SearchReportsAsync(
            ReportMapper.ToSearchReports(
                query,
                reportStatuses,
                userId,
                teamId,
                sort,
                skip,
                take,
                employeesDataAccess.DictByTeamEmployees()
                ));
        
        return searchResult.ToView(employeesDataAccess.DictEmployees());
    }
}
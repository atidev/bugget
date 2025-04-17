using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.DA.Files;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.Views;
using Bugget.Hubs;
using Bugget.Entities.Mappers;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Controllers.Obsolete;

/// <summary>
/// Api для работы с репортами
/// </summary>
[LdapAuth]
[Route("bugget/public/v1/reports")]
public sealed class ReportsController(
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
    [ProducesResponseType(typeof(ReportViewObsolete), 200)]
    public async Task<ReportViewObsolete?> CreateReportAsync([FromBody] ReportCreateDto createDto)
    {
        var user = User.GetIdentity();
        var createdReport = await reportsService.CreateReportAsync(createDto.ToReport(user.Id, user.TeamId, user.OrganizationId));

        return createdReport?.ToViewObsolete(employeesDataAccess.DictEmployees());
    }

    /// <summary>
    /// Получить репорты (для главной страницы)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ReportViewObsolete[]), 200)]
    public async Task<ReportViewObsolete[]> ListReportsAsync()
    {
        var user = User.GetIdentity();

        var reports = await reportsService.ListReportsAsync(user.Id);
        return reports.Select(r => r.ToViewObsolete(employeesDataAccess.DictEmployees())).ToArray();
    }

    /// <summary>
    /// Получить репорт
    /// </summary>
    /// <param name="reportId"></param>
    /// <returns></returns>
    [HttpGet("{reportId}")]
    [ProducesResponseType(typeof(ReportViewObsolete), 200)]
    public Task<IResult> GetReportAsync([FromRoute] int reportId)
    {
        var user = User.GetIdentity();
        return reportsService.GetReportAsync(reportId, user.OrganizationId).AsResultAsync(r => r.ToViewObsolete(employeesDataAccess.DictEmployees()));
    }

    /// <summary>
    /// Изменить репорт
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{reportId}")]
    [ProducesResponseType(typeof(ReportViewObsolete), 200)]
    public async Task<ReportViewObsolete?> UpdateReportAsync([FromRoute] int reportId, [FromBody] ReportUpdateDto updateDto)
    {
        var user = User.GetIdentity();
        var report = await reportsService.UpdateReportObsoleteAsync(updateDto.ToReportUpdate(reportId, user.Id));

        await hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReport");

        return report?.ToViewObsolete(employeesDataAccess.DictEmployees());
    }

    /// <summary>
    /// Поиск по репортам
    /// </summary>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(SearchReportsView<ReportViewObsolete>), 200)]
    public async Task<SearchReportsView<ReportViewObsolete>> SearchReportsAsync(
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
            ReportMapper.ToSearchReportsObsolete(
                query,
                reportStatuses,
                userId,
                teamId,
                sort,
                skip,
                take,
                employeesDataAccess.DictByTeamEmployees()
                ));

        return searchResult.ToViewObsolete(employeesDataAccess.DictEmployees());
    }
}
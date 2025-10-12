using Bugget.BO.Mappers;
using Bugget.BO.Services.Reports;
using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.Views;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с репортами
/// </summary>
[Route("/v1/reports")]
public sealed class ReportsObsoleteController(
    ReportsService reportsService,
    IUsersClient usersClient,
    ILogger<ReportsObsoleteController> logger) : ApiController
{
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
        return reports.Select(r => r.ToView(usersClient.DictUsers())).ToArray();
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
                take,
                usersClient.DictUsersByTeam()
                ));

        return searchResult.ToView(usersClient.DictUsers());
    }
}

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
public sealed class SearchController(
    ReportsService reportsService,
    IUsersClient usersClient) : ApiController
{
    /// <summary>
    /// Поиск по репортам
    /// </summary>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ReportViews), 200)]
    public async Task<ReportViews> SearchReportsAsync(
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

        return new ReportViews
        {
            Total = searchResult.Total,
            Reports = searchResult.Reports
        };
    }
}

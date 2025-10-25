using Bugget.BO.Services;
using Bugget.BO.Services.Reports;
using Bugget.Entities.Authentication;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Entities.Views;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;
using Bugget.DA.Interfaces;
using Bugget.BO.Mappers;
using System.ComponentModel.DataAnnotations;

namespace Bugget.Controllers;

/// <summary>
/// v2 Api для работы с репортами
/// </summary>
[Route("/v2/reports")]
public sealed class ReportsController(ReportsService reportsService, IUsersClient usersClient) : ApiController
{
    /// <summary>
    /// Создать репорт
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReportSummaryDbModel), 201)]
    public async Task<IActionResult> CreateReportAsync([FromBody] ReportV2CreateDto createDto)
    {
        var user = User.GetIdentity();
        return Ok(await reportsService.CreateReportAsync(user.Id, user.TeamId, user.OrganizationId, createDto));
    }

    /// <summary>   
    /// Получить репорт
    /// </summary>
    /// <param name="reportId"></param>
    /// <returns></returns>
    [HttpGet("{reportId:int}")]
    [ProducesResponseType(typeof(ReportDbModel), 200)]
    public Task<IActionResult> GetReportAsync([FromRoute] int reportId)
    {
        var user = User.GetIdentity();
        return reportsService.GetReportAsync(reportId, user.OrganizationId).AsActionResultAsync();
    }

    /// <summary>
    /// Частичное обновление репорта
    /// </summary>
    [HttpPatch("{reportId:int}")]
    [ProducesResponseType(typeof(ReportPatchResultDbModel), 200)]
    public Task<ReportPatchResultDbModel> PatchReportAsync([FromRoute] int reportId, [FromBody] ReportPatchDto patchDto)
    {
        var user = User.GetIdentity();
        return reportsService.PatchReportAsync(reportId, user, patchDto);
    }

    /// <summary>
    /// Получить репорты (для главной страницы)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ReportView[]), 200)]
    public async Task<ReportView[]> ListReportsAsync(
        [FromQuery] string? userId = null,
         [FromQuery] string? teamId = null,
         [FromQuery] int[]? reportStatuses = null,
         [FromQuery][Range(0, int.MaxValue)] int skip = 0,
         [FromQuery][Range(1, 100)] int take = 10)
    {
        var user = User.GetIdentity();

        var reports = await reportsService.ListReportsAsync(user.OrganizationId, userId, teamId, reportStatuses, skip, take);
        return reports.Select(r => r.ToView(usersClient.DictUsers())).ToArray();
    }
}
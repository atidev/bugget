using Authentication;
using Bugget.BO.Services;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// v2 Api для работы с репортами
/// </summary>
[Auth]
[Route("/v2/reports")]
public sealed class ReportsV2Controller(
    ReportsService reportsService) : ApiController
{
    /// <summary>
    /// Создать репорт
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReportSummaryDbModel), 200)]
    public Task<ReportSummaryDbModel> CreateReportAsync([FromBody] ReportV2CreateDto createDto)
    {
        var user = User.GetIdentity();
        return reportsService.CreateReportAsync(user.Id, user.TeamId, user.OrganizationId, createDto);
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
}
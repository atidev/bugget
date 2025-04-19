using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.Entities.DbModels.Report;
using Bugget.Entities.DTO.Report;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// v2 Api для работы с репортами
/// </summary>
[LdapAuth]
[Route("bugget/public/v2/reports")]
public sealed class ReportsV2Controller(
    ReportsService reportsService) : ApiController
{
    /// <summary>
    /// Создать репорт
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReportDbModel), 200)]
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
    [HttpGet("{reportId}")]
    [ProducesResponseType(typeof(ReportDbModel), 200)]
    public Task<IActionResult> GetReportAsync([FromRoute] int reportId)
    {
        var user = User.GetIdentity();
        return reportsService.GetReportAsync(reportId, user.OrganizationId).AsActionResultAsync();
    }

    /// <summary>
    /// Частично обновить репорт ТОЛЬКО ДЛЯ ТЕСТА (должно идти через веб-сокет)
    /// </summary>
    [HttpPatch("{reportId}")]
    [ProducesResponseType(typeof(ReportDbModel), 200)]
    public Task<ReportPatchDbModel> PatchReportAsync([FromRoute] int reportId, [FromBody] ReportPatchDto patchDto)
    {
        var user = User.GetIdentity();
        return reportsService.PatchReportAsync(reportId, user.Id, user.OrganizationId, patchDto);
    }
}
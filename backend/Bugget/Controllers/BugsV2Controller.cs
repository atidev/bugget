using Authentication;
using Bugget.BO.Services;
using Bugget.Entities.DbModels.Bug;
using Bugget.Entities.DTO.Bug;
using Bugget.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с багами
/// </summary>
[LdapAuth]
[Route("bugget/public/v2/reports/{reportId}/bugs")]
public sealed class BugsV2Controller(BugsService bugsService) : ApiController
{
    /// <summary>
    /// Добавить баг
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BugSummaryDbModel), 200)]
    public Task<IActionResult> CreateBugAsync([FromRoute] int reportId, [FromBody] BugDto createDto)
    {
        var user = User.GetIdentity();
        return bugsService.CreateBugAsync(user, reportId, createDto).AsActionResultAsync();
    }

    /// <summary>
    /// Изменить баг
    /// </summary>
    /// <returns></returns>
    [HttpPatch("{bugId}")]
    [ProducesResponseType(typeof(BugPatchResultDbModel), 200)]
    public Task<BugPatchResultDbModel> UpdateBugAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromBody] BugPatchDto patchDto)
    {
        var user = User.GetIdentity();
        return bugsService.PatchBugAsync(user, reportId, bugId, patchDto);
    }
}
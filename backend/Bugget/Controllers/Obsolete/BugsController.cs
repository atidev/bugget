using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.Entities.DTO;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.Mappers;
using Bugget.Entities.Views;
using Bugget.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Controllers.Obsolete;

/// <summary>
/// Api для работы с багами
/// </summary>
[LdapAuth]
[Route("bugget/public/v1/reports/{reportId}/bugs")]
public sealed class BugsController(BugsService bugsService,
    IHubContext<ReportPageHub> hubContext,
    EmployeesService employeesService) : ApiController
{
    /// <summary>
    /// Добавить баг
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BugViewObsolete), 200)]
    public async Task<BugViewObsolete?> CreateBugAsync([FromRoute] int reportId, [FromBody] BugDto createDto)
    {
        var user = User.GetIdentity();
        var createdBug = await bugsService.CreateBugAsync(createDto.ToBug(reportId, user.Id), user.OrganizationId);
        return createdBug.ToViewObsolete(employeesService.DictEmployees());
    }
    
    /// <summary>
    /// Изменить баг
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{bugId}")]
    [ProducesResponseType(typeof(BugViewObsolete), 200)]
    public async Task<BugViewObsolete?> UpdateBugAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromBody] BugUpdateSummaryDto updateDto)
    {
        var user = User.GetIdentity();
        var updatedBug = await bugsService.UpdateBugAsync(updateDto.ToBugUpdateSummary(reportId, bugId, user.Id), user.OrganizationId);
        
        await hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReport");

        return updatedBug?.ToViewObsolete(employeesService.DictEmployees());
    }
}
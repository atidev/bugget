using Bugget.Authentication;
using Bugget.BO.Mappers;
using Bugget.BO.Services;
using Bugget.Entities.DTO;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Bug;
using Bugget.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с багами
/// </summary>
[LdapAuth]
[Route("bugget/public/v2/reports/{reportId}/bugs")]
public sealed class BugsController(BugsService bugsService,
    IHubContext<ReportPageHub> hubContext) : ApiController
{
    /// <summary>
    /// Добавить баг
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BugView), 200)]
    public async Task<BugView?> CreateBugAsync([FromRoute] int reportId, [FromBody] BugDto createDto)
    {
        var user = User.GetIdentity();
        var createdBug = await bugsService.CreateBugAsync(createDto.ToBug(reportId, user.Id));
        return createdBug?.ToView();
    }
    
    /// <summary>
    /// Изменить баг
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <param name="updateDto"></param>
    /// <returns></returns>
    [HttpPut("{bugId}")]
    [ProducesResponseType(typeof(BugView), 200)]
    public async Task<BugView?> UpdateBugAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromBody] BugUpdateDto updateDto)
    {
        var user = User.GetIdentity();
        var updatedBug = await bugsService.UpdateBugAsync(updateDto.ToBugUpdate(reportId, bugId, user.Id));
        
        await hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReport");

        return updatedBug?.ToView();
    }

    /// <summary>
    /// Получить краткую информацию о баге
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="bugId"></param>
    /// <returns></returns>
    [HttpGet("{bugId}/summary")]
    [ProducesResponseType(typeof(BugSummaryView), 200)]
    public async Task<BugSummaryView?> GetBugSummaryAsync([FromRoute] int reportId, [FromRoute] int bugId)
    {
        var user = User.GetIdentity();
        var bug = await bugsService.GetBugSummaryAsync(reportId, bugId, user.OrganizationId);
        return bug?.ToSummaryView();
    }
} 
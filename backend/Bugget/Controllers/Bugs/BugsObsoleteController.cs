using Bugget.BO.Mappers;
using Bugget.BO.Services;
using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.DTO.Bug;
using Bugget.Entities.Views;
using Bugget.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с багами
/// </summary>
[Route("/v1/reports/{reportId}/bugs")]
public sealed class BugsObsoleteController(BugsService bugsService,
    IHubContext<ReportPageHub> hubContext,
    IEmployeesClient employeesClient) : ApiController
{
    /// <summary>
    /// Добавить баг
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="createDto"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BugView), 200)]
    public async Task<BugView?> CreateBugAsync([FromRoute] int reportId, [FromBody] BugDtoObsolete createDto)
    {
        var user = User.GetIdentity();
        var createdBug = await bugsService.CreateBugObsoleteAsync(createDto.ToBug(reportId, user.Id));
        return createdBug?.ToView(employeesClient.DictEmployees());
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
    public async Task<BugView?> UpdateBugAsync([FromRoute] int reportId, [FromRoute] int bugId, [FromBody] BugUpdateDtoObsolete updateDto)
    {
        var user = User.GetIdentity();
        var updatedBug = await bugsService.UpdateBugObsoleteAsync(updateDto.ToBugUpdate(reportId, bugId, user.Id));
        
        await hubContext.Clients.Group($"{reportId}")
            .SendAsync("ReceiveReport");

        return updatedBug?.ToView(employeesClient.DictEmployees());
    }
}
using System.ComponentModel.DataAnnotations;
using Bugget.BO.Services;
using Bugget.DA.Files;
using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.Views;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с командами
/// </summary>
[Route("/v1/teams")]
public sealed class TeamsController(ITeamsClient teamsClient) : ApiController
{
    /// <summary>
    /// Поиск команды по имени
    /// </summary>
    [HttpGet("autocomplete")]
    [ProducesResponseType(typeof(FoundedTeamsView), 200)]
    public IActionResult AutocompleteTeams([FromQuery] [Required] string searchString,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] uint depth = 1)
    {
        if (string.IsNullOrWhiteSpace(searchString) || skip < 0 || take <= 0)
            return BadRequest();

        var user = User.GetIdentity();
        if(string.IsNullOrEmpty(user.TeamId))
            return Ok(new FoundedTeamsView{
                Teams = [],
                Total = 0
            });

        var (teams, total) = teamsClient.AutocompleteTeams(
            user.TeamId,
            searchString,
            skip,
            take,
            depth);

        return Ok(new FoundedTeamsView
        {
            Teams = teams.Select(t => new TeamView
            {
                Id = t.Id,
                Name = t.Name
            }),
            Total = total
        });
    }
}
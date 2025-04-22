using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Authentication;
using Bugget.BO.Services;
using Bugget.Entities.Constants;
using Bugget.Entities.Views;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с командами
/// </summary>
[LdapAuth]
[Route("bugget/public/v1/teams")]
public sealed class TeamsController(EmployeesService service) : ApiController
{
    /// <summary>
    /// Поиск команды по имени
    /// </summary>
    [HttpGet("autocomplete")]
    [ProducesResponseType(typeof(FoundedTeamsView), 200)]
    public IActionResult AutocompleteEmployees([FromQuery] [Required] string searchString,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] uint depth = 1)
    {
        if (string.IsNullOrWhiteSpace(searchString) || skip < 0 || take <= 0)
            return BadRequest();

        var user = User.GetIdentity();
        var (teams, total) = service.AutocompleteTeams(
            user.Id,
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
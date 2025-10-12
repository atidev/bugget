using System.ComponentModel.DataAnnotations;
using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.Views.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с пользователями
/// </summary>
[Route("/v1/users")]
public sealed class UsersController(IUsersClient usersClient) : ApiController
{
    /// <summary>
    /// Поиск пользователей по имени
    /// </summary>
    [HttpGet("autocomplete")]
    [ProducesResponseType(typeof(AutocompleteUsersView), 200)]
    public async Task<IActionResult> AutocompleteUsers([FromQuery] [Required] string searchString,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] uint depth = 1)
    {
        if (string.IsNullOrWhiteSpace(searchString) || skip < 0 || take <= 0)
            return BadRequest();

        var user = User.GetIdentity();
        var (users, total) = await usersClient.AutocompleteUsersAsync(
            user.Id,
            searchString,
            skip,
            take,
            depth);

        return Ok(new AutocompleteUsersView
        {
            Users = users.Select(e => new UserView
            {
                Id = e.Id,
                Name = e.Name,
                PhotoUrl = e.PhotoUrl,
                TeamId = e.TeamId
            }),
            Total = total
        });
    }

    /// <summary>
    /// Получить информацию о пользователях по их идентификаторам
    /// </summary>
    [HttpPost("batch/list")]
    [ProducesResponseType(typeof(IEnumerable<UserView>), 200)]
    public async Task<IActionResult> GetUserViewsAsync([FromBody] string[] userIds)
    {
        if (userIds == null || userIds.Length == 0)
            return BadRequest();

        var user = User.GetIdentity();
        var users = await usersClient.GetUsersAsync(userIds, user.OrganizationId);
        return Ok(users.Select(e => new UserView { Id = e.Id, Name = e.Name, PhotoUrl = e.PhotoUrl, TeamId = e.TeamId }));
    }

    /// <summary>
    /// Получить данные текущего пользователя
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = User.GetIdentity();
        
        var userDbModel = await usersClient.GetUserAsync(user.Id);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new UserAuthView
        {
            Id = userDbModel.Id,
            Name = userDbModel.Name,
            PhotoUrl = userDbModel.PhotoUrl,
            TeamId = userDbModel.TeamId 
        });
    }
}
using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.Views.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

[Route("/v1/auth")]
public sealed class AuthController(IUsersClient usersClient) : ApiController
{
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
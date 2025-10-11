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
        
        var user = await usersClient.GetUserAsync(user.Id);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new UserAuthView
        {
            Id = user.Id,
            Name = user.Name,
            PhotoUrl = user.PhotoUrl,
            TeamId = user.TeamId 
        });
    }
}
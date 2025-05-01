using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.Views.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

[Route("/v1/auth")]
public sealed class AuthController(IEmployeesClient employeesClient) : ApiController
{
    /// <summary>
    /// Получить данные текущего пользователя
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = User.GetIdentity();
        
        var employee = await employeesClient.GetEmployeeAsync(user.Id);
        if (employee == null)
        {
            return Unauthorized();
        }

        return Ok(new UserAuthView
        {
            Id = employee.Id,
            Name = employee.Name,
            PhotoUrl = employee.PhotoUrl,
            TeamId = user.TeamId 
        });
    }
}
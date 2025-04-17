using Bugget.Authentication;
using Bugget.DA.Files;
using Bugget.Entities.Mappers;
using Bugget.Entities.Views.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

[LdapAuth]
[Route("bugget/public/v1/auth")]
public sealed class AuthController(EmployeesDataAccess employeesDataAccess) : ApiController
{
    /// <summary>
    /// Получить данные текущего пользователя
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetCurrentUser()
    {
        var user = User.GetIdentity();
        if (!employeesDataAccess.DictEmployees().TryGetValue(user.Id, out var employee))
        {
            return BadRequest("user not found");
        }

        return Ok(new UserAuthView
        {
            Id = employee.Id,
            Name = EmployeesMapper.Transform(employee).Name,
            TeamId = employee.TeamId
        });
    }
}
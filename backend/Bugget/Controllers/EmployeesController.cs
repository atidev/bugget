using System.ComponentModel.DataAnnotations;
using Authentication;
using Bugget.BO.Services;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с сотрудниками
/// </summary>
[Auth]
[Route("bugget/public/v1/employees")]
public sealed class EmployeesController(EmployeesService service) : ApiController
{
    /// <summary>
    /// Поиск сотрудников по имени
    /// </summary>
    [HttpGet("autocomplete")]
    [ProducesResponseType(typeof(FoundedEmployeesView), 200)]
    public IActionResult AutocompleteEmployees([FromQuery] [Required] string searchString,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] uint depth = 1)
    {
        if (string.IsNullOrWhiteSpace(searchString) || skip < 0 || take <= 0)
            return BadRequest();

        var user = User.GetIdentity();
        var (employees, total) = service.AutocompleteEmployees(
            user.Id,
            searchString,
            skip,
            take,
            depth);
        
        return Ok(new FoundedEmployeesView { Employees = employees, Total = total });
    }

    /// <summary>
    /// Получить информацию о пользователях по их идентификаторам
    /// </summary>
    [HttpPost("users")]
    [ProducesResponseType(typeof(IEnumerable<UserView>), 200)]
    public async Task<IActionResult> GetUserViewsAsync([FromBody] string[] userIds)
    {
        if (userIds == null || userIds.Length == 0)
            return BadRequest();

        var user = User.GetIdentity();
        var employees = await service.GetUserViewsAsync(userIds, user.OrganizationId);
        return Ok(employees);
    }
}
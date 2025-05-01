using System.ComponentModel.DataAnnotations;
using Bugget.BO.Services;
using Bugget.DA.Interfaces;
using Bugget.Entities.Authentication;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Users;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с сотрудниками
/// </summary>
[Route("/v1/employees")]
public sealed class EmployeesController(IEmployeesClient employeesClient) : ApiController
{
    /// <summary>
    /// Поиск сотрудников по имени
    /// </summary>
    [HttpGet("autocomplete")]
    [ProducesResponseType(typeof(FoundedEmployeesView), 200)]
    public async Task<IActionResult> AutocompleteEmployees([FromQuery] [Required] string searchString,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] uint depth = 1)
    {
        if (string.IsNullOrWhiteSpace(searchString) || skip < 0 || take <= 0)
            return BadRequest();

        var user = User.GetIdentity();
        var (employees, total) = await employeesClient.AutocompleteEmployeesAsync(
            user.Id,
            searchString,
            skip,
            take,
            depth);

        return Ok(new FoundedEmployeesView
        {
            Employees = employees.Select(e => new EmployeeView
            {
                Id = e.Id,
                Name = e.Name
            }),
            Total = total
        });
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
        var employees = await employeesClient.GetEmployeesAsync(userIds, user.OrganizationId);
        return Ok(employees.Select(e => new UserView { Id = e.Id, Name = e.Name, PhotoUrl = e.PhotoUrl, TeamId = e.TeamId }));
    }
}
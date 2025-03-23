using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Bugget.Authentication;
using Bugget.BO.Services;
using Bugget.Entities.Constants;
using Bugget.Entities.Views;
using Microsoft.AspNetCore.Mvc;

namespace Bugget.Controllers;

/// <summary>
/// Api для работы с сотрудниками
/// </summary>
[LdapAuth]
[Route("bugget/public/v1/employees")]
public sealed class EmployeesController(EmployeesService service) : ApiController
{
    /// <summary>
    /// Получить список сотрудников
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(FoundedEmployeesView), 200)]
    public IActionResult ListEmployees([FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        if (skip < 0 || take <= 0)
            return BadRequest();
        
        var (employees, total) = service.ListEmployees(skip, take);
        
        return Ok(new FoundedEmployeesView { Employees = employees, Total = total });
    }
    
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
}
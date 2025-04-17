using Bugget.DA.Files;
using Bugget.DA.Interfaces;
using Bugget.Entities.BO;
using Bugget.Entities.Mappers;
using Bugget.Entities.Views;
using Bugget.Entities.Views.Users;

namespace Bugget.BO.Services;

public class EmployeesService(EmployeesDataAccess employeesDataAccess, IEmployeesClient employeesClient)
{

    public (IEnumerable<EmployeeView>, int) AutocompleteEmployees(
        string userId,
        string searchString,
        int skip,
        int take,
        uint depth)
    {
        var user = employeesDataAccess.GetEmployee(userId);
        if (user == null)
            return ([], 0);

        var foundedUsers = employeesDataAccess.ListEmployees()
            // текущая глубина + 1
            .Where(e => user.Depth == null || e.Depth >= user.Depth - depth)
            .Select(EmployeesMapper.Transform)
            .Where(v => v.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .OrderBy(v => v.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase))
            .ThenBy(v => v.Name)
            .ToArray();

        return (foundedUsers.Skip(skip).Take(take), foundedUsers.Length);
    }
    
    public (IEnumerable<Team>, int) AutocompleteTeams(
        string userId,
        string searchString,
        int skip,
        int take,
        uint depth)
    {
        var user = employeesDataAccess.GetEmployee(userId);
        if (user == null)
            return ([], 0);

        var foundedTeams = employeesDataAccess.ListTeams()
            // текущая глубина + 1
            .Where(e => user.Depth == null || e.Depth >= user.Depth - depth)
            .Where(v => v.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .OrderBy(v => v.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase))
            .ThenBy(v => v.Name)
            .ToArray();

        return (foundedTeams.Skip(skip).Take(take), foundedTeams.Length);
    }

    public IReadOnlyDictionary<string, EmployeeObsolete> DictEmployees()
    {
        return employeesDataAccess.DictEmployees();
    }

    public async Task<IEnumerable<UserView>> GetUserViewsAsync(IEnumerable<string> userIds, string? organizationId)
    {
        var employees = await employeesClient.GetEmployeesAsync(userIds, organizationId);
        return employees.Select(e => new UserView { Id = e.Id, Name = e.Name, PhotoUrl = e.PhotoUrl });
    }
}
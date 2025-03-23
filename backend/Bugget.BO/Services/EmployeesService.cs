using Bugget.DA.Files;
using Bugget.Entities.Adapters;
using Bugget.Entities.BO;
using Bugget.Entities.Constants;
using Bugget.Entities.Views;

namespace Bugget.BO.Services;

public class EmployeesService(EmployeesDataAccess employeesDataAccess)
{
    public (IEnumerable<EmployeeView>, int) ListEmployees(int skip, int take)
    {
        return (employeesDataAccess.ListEmployees().Skip(skip).Take(take).Select(EmployeeAdapter.Transform),
            employeesDataAccess.ListEmployees().Count);
    }

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
            .Select(EmployeeAdapter.Transform)
            .Where(v => v.FullName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .OrderBy(v => v.FullName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase))
            .ThenBy(v => v.FullName)
            .ToList();

        return (foundedUsers.Skip(skip).Take(take), foundedUsers.Count);
    }

    public IReadOnlyDictionary<string, Employee> DictEmployees()
    {
        return employeesDataAccess.DictEmployees();
    }
}
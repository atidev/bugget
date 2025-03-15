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
        return (employeesDataAccess.ListEmployees().Skip(skip).Take(take).Select(EmployeeAdapter.Transform), employeesDataAccess.ListEmployees().Count);
    }

    public (IEnumerable<EmployeeView>, int) AutocompleteEmployees(string searchString, int skip, int take)
    {
        var foundedUsers = employeesDataAccess.ListEmployees()
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
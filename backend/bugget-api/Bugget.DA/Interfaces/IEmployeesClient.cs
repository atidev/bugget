using Bugget.Entities.BO;
using Bugget.Entities.Views;

namespace Bugget.DA.Interfaces;

public interface IEmployeesClient
{
    Task<Employee?> GetEmployeeAsync(string userId);
    Task<IEnumerable<Employee>> GetEmployeesAsync(IEnumerable<string> userIds, string? organizationId);
    
    [Obsolete]
    IReadOnlyDictionary<string, Employee> DictEmployees();

    [Obsolete]
    public IReadOnlyDictionary<string, IReadOnlyCollection<Employee>> DictEmployeesByTeam();
    
    public Task<(IEnumerable<Employee>, int)> AutocompleteEmployeesAsync(
        string userId,
        string searchString,
        int skip,
        int take,
        uint depth);
}
using Bugget.Entities.BO;

namespace Bugget.DA.Interfaces;

public interface IEmployeesClient
{
    Task<Employee?> GetEmployeeAsync(string userId);
    Task<IEnumerable<Employee>> GetEmployeesAsync(IEnumerable<string> userIds);
} 
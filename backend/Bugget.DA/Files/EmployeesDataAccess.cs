using Bugget.Entities.BO;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Bugget.DA.Files;

public sealed class EmployeesDataAccess(ILogger<EmployeesDataAccess> logger) : BackgroundService
{
    public Employee? GetEmployee(string userId)
    {
        EmployeesDict.Value.TryGetValue(userId, out var employee);

        return employee;
    }

    public IReadOnlyDictionary<string, Employee> DictEmployees()
    {
        return EmployeesDict.Value;
    }

    public IReadOnlyCollection<Employee> ListEmployees()
    {
        return EmployeesCollection.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return LoadEmployees();
    }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private async Task LoadEmployees()
    {
        string employeesFilePath = Path.Combine(AppContext.BaseDirectory, "employees.json");

        if (File.Exists(employeesFilePath))
        {
            Employees = JsonSerializer.Deserialize<Employee[]>(await File.ReadAllTextAsync(employeesFilePath), JsonSerializerOptions)
                        ?? throw new InvalidOperationException("Ошибка загрузки данных из employees.json");
            return;
        }

        logger.LogWarning("Файл employees.json не найден, используются данные по умолчанию");
    }

    private static Employee[] Employees =
    [
        new Employee { Id = "1", FirstName = "Иванов", LastName = "Иван", Surname = "Иванович", NotificationUserId = "66xpfgxex2da4p5fn8dx17pcnr" },
        new Employee
        {
            Id = "any-ldap-id", FirstName = "Петров", LastName = "Петр", Surname = "Петрович", NotificationUserId = "67xpfgxex2da4p5fn8dx17pcnr"
        },
        new Employee
        {
            Id = "int", FirstName = "Сергеев", LastName = "Сергей", Surname = "Сергеевич", NotificationUserId = "68xpfgxex2da4p5fn8dx17pcnr"
        },
        new Employee
        {
            Id = "guid", FirstName = "Алексеев", LastName = "Алексей", Surname = "Алексеевич", NotificationUserId = "69xpfgxex2da4p5fn8dx17pcnr"
        }
    ];

    private static readonly Lazy<IReadOnlyCollection<Employee>> EmployeesCollection = new(() => Employees.AsReadOnly());

    private static readonly Lazy<IReadOnlyDictionary<string, Employee>> EmployeesDict = new(() => Employees.ToDictionary(k => k.Id).AsReadOnly());
}
using Bugget.Entities.BO;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Bugget.DA.Files;

public sealed class EmployeesDataAccess : BackgroundService
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

    private static async Task LoadEmployees()
    {
        string employeesFilePath = Path.Combine(AppContext.BaseDirectory, "employees.json");
        string defaultFilePath = Path.Combine(AppContext.BaseDirectory, "employees.default.json");

        if (File.Exists(employeesFilePath))
        {
            Employees = JsonSerializer.Deserialize<Employee[]>(await File.ReadAllTextAsync(employeesFilePath), JsonSerializerOptions)
                        ?? throw new InvalidOperationException("Ошибка загрузки данных из employees.json");
            return;
        }

        if (File.Exists(defaultFilePath))
        {
            Employees = JsonSerializer.Deserialize<Employee[]>(await File.ReadAllTextAsync(defaultFilePath), JsonSerializerOptions)
                        ?? throw new InvalidOperationException("Ошибка загрузки данных из employees.default.json");
            return;
        }

        throw new FileNotFoundException("Файл с сотрудниками не найден");
    }

    private static Employee[] Employees = [];

    private static readonly Lazy<IReadOnlyCollection<Employee>> EmployeesCollection = new(()=>Employees.AsReadOnly());

    private static readonly Lazy<IReadOnlyDictionary<string, Employee>> EmployeesDict = new(()=>Employees.ToDictionary(k => k.Id).AsReadOnly());
}
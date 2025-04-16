using Bugget.DA.Interfaces;
using Bugget.Entities.BO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Bugget.DA.Files;

public sealed class EmployeesFileClient(ILogger<EmployeesFileClient> logger) : BackgroundService, IEmployeesClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private static Employee[] _employees = Array.Empty<Employee>();
    private static readonly Lazy<IReadOnlyDictionary<string, Employee>> EmployeesDict = new(() => _employees.ToDictionary(k => k.Id));


    public Task<Employee?> GetEmployeeAsync(string userId)
    {
        EmployeesDict.Value.TryGetValue(userId, out var employee);
        return Task.FromResult(employee);
    }

    public Task<IEnumerable<Employee>> GetEmployeesAsync(IEnumerable<string> userIds)
    {
        var employees = userIds
            .Select(id => EmployeesDict.Value.TryGetValue(id, out var employee) ? employee : null)
            .Where(e => e != null);
        return Task.FromResult(employees!);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return LoadEmployeesAsync();
    }

    private async Task LoadEmployeesAsync()
    {
        string employeesFilePath = Path.Combine(AppContext.BaseDirectory, "employees.json");

        if (File.Exists(employeesFilePath))
        {
            _employees = JsonSerializer.Deserialize<Employee[]>(await File.ReadAllTextAsync(employeesFilePath), JsonSerializerOptions)
                        ?? throw new InvalidOperationException("Ошибка загрузки данных из employees.json");
            return;
        }

        logger.LogWarning("Файл employees.json не найден, используются данные по умолчанию");
        _employees = GetDefaultEmployees();
    }

    private static Employee[] GetDefaultEmployees()
    {
        return
        [
            new Employee
            {
                Id = "1",
                Name = "Иванов Иван Иванович",
                NotificationUserId = "66xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "4",
                OrganizationId = "1",
                Depth = -1
            },
            new Employee
            {
                Id = "any-ldap-id",
                Name = "Петров Петр Петрович",
                NotificationUserId = "67xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "4",
                OrganizationId = "1",
                Depth = 0
            },
            new Employee
            {
                Id = "int",
                Name = "Сергей Сергеев Сергеевич",
                NotificationUserId = "68xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "3",
                OrganizationId = "1",
                Depth = 2
            },
            new Employee
            {
                Id = "guid",
                Name = "Алексей Алексеев Алексеевич",
                NotificationUserId = "69xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "2",
                OrganizationId = "1",
                Depth = 1
            },
            new Employee
            {
                Id = "default-user",
                Name = "Дефолт Дефолтов Дефолтович",
                NotificationUserId = "69xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "1",
                OrganizationId = "1",
                Depth = 1
            }
        ];
    }
}
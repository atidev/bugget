using Bugget.Entities.BO;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Bugget.DA.Files;

public sealed class EmployeesDataAccess(ILogger<EmployeesDataAccess> logger) : BackgroundService
{
    public EmployeeObsolete? GetEmployee(string userId)
    {
        EmployeesDict.Value.TryGetValue(userId, out var employee);

        return employee;
    }

    public IReadOnlyDictionary<string, EmployeeObsolete> DictEmployees()
    {
        return EmployeesDict.Value;
    }

    public IReadOnlyCollection<EmployeeObsolete> ListEmployees()
    {
        return EmployeesCollection.Value;
    }

    public IReadOnlyCollection<Team> ListTeams()
    {
        return TeamsCollection.Value;
    }

    public IReadOnlyDictionary<string, IReadOnlyCollection<EmployeeObsolete>> DictByTeamEmployees()
    {
        return EmployeesByTeam.Value;
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
            Employees = JsonSerializer.Deserialize<EmployeeObsolete[]>(await File.ReadAllTextAsync(employeesFilePath), JsonSerializerOptions)
                        ?? throw new InvalidOperationException("Ошибка загрузки данных из employees.json");
            return;
        }

        logger.LogWarning("Файл employees.json не найден, используются данные по умолчанию");
    }

    private static EmployeeObsolete[] Employees =
    [
        new EmployeeObsolete
        {
            Id = "1",
            FirstName = "Иван",
            LastName = "Иванов",
            Surname = "Иванович",
            NotificationUserId = "66xpfgxex2da4p5fn8dx17pcnr",
            TeamId = "4",
            TeamName = "test4",
            Depth = -1
        },
        new EmployeeObsolete
        {
            Id = "any-id-1",
            FirstName = "Петр",
            LastName = "Петров",
            Surname = "Петрович",
            NotificationUserId = "67xpfgxex2da4p5fn8dx17pcnr",
            TeamId = "4",
            TeamName = "test4",
            Depth = 0
        },
        new EmployeeObsolete
        {
            Id = "int",
            FirstName = "Сергей",
            LastName = "Сергеев",
            Surname = "Сергеевич",
            NotificationUserId = "68xpfgxex2da4p5fn8dx17pcnr",
            TeamId = "3",
            TeamName = "test3",
            Depth = 2
        },
        new EmployeeObsolete
        {
            Id = "guid",
            FirstName = "Алексей",
            LastName = "Алексеев",
            Surname = "Алексеевич",
            NotificationUserId = "69xpfgxex2da4p5fn8dx17pcnr",
            TeamId = "2",
            TeamName = "test2",
            Depth = 1
        },
        new EmployeeObsolete
        {
            Id = "default-user",
            FirstName = "Дефолт",
            LastName = "Дефолтов",
            Surname = "Дефолтович",
            NotificationUserId = "69xpfgxex2da4p5fn8dx17pcnr",
            TeamId = "1",
            TeamName = "test1",
            Depth = 1
        }
    ];

    private static readonly Lazy<IReadOnlyCollection<EmployeeObsolete>> EmployeesCollection = new(() => Employees);

    private static readonly Lazy<IReadOnlyCollection<Team>> TeamsCollection = new(() =>
    {
        if (Employees.Any(e => string.IsNullOrEmpty(e.TeamId) || string.IsNullOrEmpty(e.TeamName)))
            return [];

        return Employees.GroupBy(e => e.TeamId).Select(g =>
        {
            var anyEmpl = g.First();
            return new Team
            {
                Id = anyEmpl.TeamId!,
                Name = anyEmpl.TeamName!,
                Depth = anyEmpl.Depth
            };
        }).ToArray();
    });

    private static readonly Lazy<IReadOnlyDictionary<string, EmployeeObsolete>> EmployeesDict = new(() => Employees.ToDictionary(k => k.Id));

    private static readonly Lazy<IReadOnlyDictionary<string, IReadOnlyCollection<EmployeeObsolete>>> EmployeesByTeam = new(() =>
        Employees
            .GroupBy(e => e.TeamId ?? string.Empty)
            .ToDictionary(
                g => g.Key, IReadOnlyCollection<EmployeeObsolete> (g) => g.ToArray()
            ));
}
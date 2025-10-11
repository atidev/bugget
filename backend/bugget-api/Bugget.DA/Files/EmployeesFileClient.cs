using Bugget.DA.Interfaces;
using Bugget.Entities.BO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Bugget.DA.Files;

public sealed class UsersFileClient(ILogger<UsersFileClient> logger) : BackgroundService, IUsersClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private static User[] _users = [];
    private static readonly Lazy<IReadOnlyDictionary<string, User>> UsersDict = new(() => _users.ToDictionary(k => k.Id));


    public Task<User?> GetUserAsync(string userId)
    {
        UsersDict.Value.TryGetValue(userId, out var users);
        return Task.FromResult(users);
    }

    public Task<IEnumerable<User>> GetUsersAsync(IEnumerable<string> userIds, string? organizationId)
    {
        var users = userIds
            .Select(id => UsersDict.Value.GetValueOrDefault(id))
            .Where(e => e != null);
        return Task.FromResult(users!);
    }

    public IReadOnlyDictionary<string, User> DictUsers()
    {
        return UsersDict.Value;
    }
    
    public IReadOnlyDictionary<string, IReadOnlyCollection<User>> DictUsersByTeam()
    {
        return _users
            .Where(e => !string.IsNullOrEmpty(e.TeamId))
            .GroupBy(e => e.TeamId!)
            .ToDictionary(
                grp => grp.Key!,
                grp => (IReadOnlyCollection<User>)grp
                    .Select(e => new User
                    {
                        Id = e.Id,
                        Name = e.Name,
                        NotificationUserId = e.NotificationUserId,
                        TeamId = e.TeamId,
                        OrganizationId = e.OrganizationId,
                        PhotoUrl = e.PhotoUrl,
                        Depth = e.Depth
                    })
                    .ToList()
            );
    }
    
    public async Task<(IEnumerable<User>, int)> AutocompleteUsersAsync(
        string userId,
        string searchString,
        int skip,
        int take,
        uint depth)
    {
        var user = await GetUserAsync(userId);
        if (user == null)
            return ([], 0);

        var foundedUsers = _users
            // текущая глубина + 1
            .Where(e => user.Depth == null || e.Depth >= user.Depth - depth)
            .Where(v => v.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .OrderBy(v => v.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase))
            .ThenBy(v => v.Name)
            .ToArray();

        return (foundedUsers.Skip(skip).Take(take), foundedUsers.Length);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        string usersFilePath = Path.Combine(AppContext.BaseDirectory, "employees.json");

        if (File.Exists(usersFilePath))
        {
            _users = JsonSerializer.Deserialize<User[]>(await File.ReadAllTextAsync(usersFilePath), JsonSerializerOptions)
                        ?? throw new InvalidOperationException("Ошибка загрузки данных из employees.json");
            return;
        }

        logger.LogWarning("Файл employees.json не найден, используются данные по умолчанию");
        _users = GetDefaultUsers();
    }

    private static User[] GetDefaultUsers()
    {
        return
        [
            new User
            {
                Id = "1",
                Name = "Иванов Иван Иванович",
                NotificationUserId = "66xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "4",
                OrganizationId = "1",
                Depth = -1
            },
            new User
            {
                Id = "any-id-1",
                Name = "Петров Петр Петрович",
                NotificationUserId = "67xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "4",
                OrganizationId = "1",
                Depth = 0
            },
            new User
            {
                Id = "int",
                Name = "Сергей Сергеев Сергеевич",
                NotificationUserId = "68xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "3",
                OrganizationId = "1",
                Depth = 2
            },
            new User
            {
                Id = "guid",
                Name = "Алексей Алексеев Алексеевич",
                NotificationUserId = "69xpfgxex2da4p5fn8dx17pcnr",
                TeamId = "2",
                OrganizationId = "1",
                Depth = 1
            },
            new User
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
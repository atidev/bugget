using Bugget.DA.Interfaces;
using Bugget.Entities.BO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Bugget.DA.Files;

public sealed class TeamsFileClient(ILogger<UsersFileClient> logger) : BackgroundService, ITeamsClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    private static Team[] _teams = [];
    private static readonly Lazy<IReadOnlyDictionary<string, Team>> TeamsDict = new(() => _teams.ToDictionary(k => k.Id));


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return LoadTeamsAsync();
    }

    private async Task LoadTeamsAsync()
    {
        string teamsFilePath = Path.Combine(AppContext.BaseDirectory, "teams.json");

        if (File.Exists(teamsFilePath))
        {
            _teams = JsonSerializer.Deserialize<Team[]>(await File.ReadAllTextAsync(teamsFilePath), JsonSerializerOptions)
                         ?? throw new InvalidOperationException("Ошибка загрузки данных из teams.json");
            return;
        }

        logger.LogWarning("Файл teams.json не найден, используются данные по умолчанию");
        _teams = GetDefaultTeams();
    }

    private static Team[] GetDefaultTeams()
    {
        return
        [
            new Team
            {
                Id = "4",
                Name = "test4",
                Depth = 0
            },
            new Team
            {
                Id = "3",
                Name = "test3",
                Depth = 2
            },
            new Team
            {
                Id = "2",
                Name = "test2",
                Depth = 1
            },
            new Team
            {
                Id = "1",
                Name = "test1",
                Depth = 1
            }
        ];
    }

    public (IEnumerable<Team>, int) AutocompleteTeams(
        string searchUserTeamId,
        string searchString,
        int skip,
        int take,
        uint depth)
    {
        var team = TeamsDict.Value.GetValueOrDefault(searchUserTeamId);
        if (team == null)
            return ([], 0);

        var foundedTeams = _teams
            // текущая глубина + 1
            .Where(e => team.Depth == null || e.Depth >= team.Depth - depth)
            .Where(v => v.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            .OrderBy(v => v.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase))
            .ThenBy(v => v.Name)
            .ToArray();

        return (foundedTeams.Skip(skip).Take(take), foundedTeams.Length);
    }
}
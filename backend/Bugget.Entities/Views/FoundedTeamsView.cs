namespace Bugget.Entities.Views;

public class FoundedTeamsView
{
    public required IEnumerable<TeamView> Teams { get; init; }
    public required int Total { get; init; }
}
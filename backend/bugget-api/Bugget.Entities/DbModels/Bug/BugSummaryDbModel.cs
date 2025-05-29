namespace Bugget.Entities.DbModels.Bug;

public sealed class BugSummaryDbModel
{
    public required int Id { get; init; }
    public string? Receive { get; init; }
    public string? Expect { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string CreatorUserId { get; init; }
    public required int Status { get; init; }
}
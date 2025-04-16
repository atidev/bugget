namespace Bugget.Entities.BO.Search;

public sealed class SearchReports
{
    public required string? Query { get; init; }
    public required int[]? ReportStatuses { get; init; }
    public required string? TeamId { get; init; }
    public required string? UserId { get; init; }
    public required string? OrganizationId { get; init; }
    public required SortOption Sort { get; init; }
    public required uint Skip { get; init; }
    public required uint Take { get; init; }
}
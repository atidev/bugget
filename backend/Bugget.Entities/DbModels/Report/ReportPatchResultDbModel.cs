namespace Bugget.Entities.DbModels.Report;

public sealed class ReportPatchResultDbModel
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string ResponsibleUserId { get; init; }
    public required string PastResponsibleUserId { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
}
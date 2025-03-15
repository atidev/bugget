namespace Bugget.Entities.DTO.Report;

public sealed class ReportUpdateDto
{
    public string? Title { get; init; }
    public int? Status { get; init; }
    public string? ResponsibleUserId { get; init; }
}
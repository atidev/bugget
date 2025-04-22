namespace Bugget.Entities.DTO.Report;

public sealed class ReportCreateDto
{
    public required string Title { get; init; }
    public required string ResponsibleId { get; init; }
    public required BugDtoObsolete[] Bugs { get; init; }
}
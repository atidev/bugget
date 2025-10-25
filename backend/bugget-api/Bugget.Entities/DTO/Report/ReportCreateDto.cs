using Bugget.Entities.DTO.Bug;

namespace Bugget.Entities.DTO.Report;

public sealed class ReportCreateDto
{
    public required string Title { get; init; }
    public required string ResponsibleId { get; init; }
    public required BugDto[] Bugs { get; init; }
}
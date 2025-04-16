namespace Bugget.Entities.DTO.Report;

public sealed class ReportUpdateDto
{
    public required string Title { get; init; }
    public required string ResponsibleUserId { get; init; }
    public required string[] ParticipantsUserIds { get; init; }
    public required int Status { get; init; }
}
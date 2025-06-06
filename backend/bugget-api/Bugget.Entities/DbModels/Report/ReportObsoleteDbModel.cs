using Bugget.Entities.DbModels.Bug;

namespace Bugget.Entities.DbModels.Report;

public sealed class ReportObsoleteDbModel
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string ResponsibleUserId { get; init; }
    public required string CreatorUserId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string[] ParticipantsUserIds { get; init; } 
    public required BugDbModel[] Bugs { get; init; }
}
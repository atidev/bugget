using Bugget.Entities.DbModels.Bug;

namespace Bugget.Entities.DbModels.Report;

public sealed class ReportCreateDbModel
{
    public required string Title { get; init; }
    public required int Status { get; init; }
    public required string ResponsibleUserId { get; init; }
    public required string CreatorUserId { get; init; }
    public required string[] ParticipantsUserIds { get; init; } 
    public required BugCreateDbModel[] Bugs { get; init; }
}
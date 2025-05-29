namespace Bugget.Entities.DbModels;

public sealed class ReportUpdateDbModel
{
    public required int Id { get; init; }
    public string? Title { get; init; }
    public int? Status { get; init; }
    public string? ResponsibleUserId { get; init; }
    public required string[] ParticipantsUserIds { get; init; } 
}
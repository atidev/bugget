namespace Bugget.Entities.BO.ReportBo;

public sealed class Report
{
    public int? Id { get; set; }
    public required string Title { get; set; }
    public required string CreatorUserId { get; set; }
    public string? CreatorTeamId { get; set; }
    public string? CreatorOrganizationId { get; set; }
    public required string ResponsibleUserId { get; set; }
    public required string[] ParticipantsUserIds { get; set; }
    public required int Status { get; set; }
    public Bug[] Bugs { get; set; } = Array.Empty<Bug>();
}
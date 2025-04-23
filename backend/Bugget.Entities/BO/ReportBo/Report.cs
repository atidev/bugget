namespace Bugget.Entities.BO.ReportBo;

public sealed class Report
{
    public int? Id { get; set; }
    public required string Title { get; set; }
    public int Status { get; set; } = (int)ReportStatus.Backlog;
    public required string ResponsibleUserId { get; set; }
    public required string CreatorUserId { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public required string[] ParticipantsUserIds { get; set; }
    
    public required Bug[] Bugs { get; set; }
}
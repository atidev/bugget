namespace Bugget.Entities.BO.ReportBo;

public sealed class ReportUpdate
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string UpdaterUserId { get; set; }
    public required string ResponsibleUserId { get; set; }
    public required string[] ParticipantsUserIds { get; set; }
    public required int Status { get; set; }
}
namespace Bugget.Entities.SocketView;

public class ReportSocketView
{
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? ResponsibleUserId { get; set; }
    public string? PastResponsibleUserId { get; set; }
    public string[]? ParticipantsUserIds { get; set; }
}
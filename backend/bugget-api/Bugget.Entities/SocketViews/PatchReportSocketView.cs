namespace Bugget.Entities.SocketViews;

public class PatchReportSocketView
{
    public string? Title { get; set; }
    public int? Status { get; set; }
    public string? ResponsibleUserId { get; set; }
    public string? PastResponsibleUserId { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
}
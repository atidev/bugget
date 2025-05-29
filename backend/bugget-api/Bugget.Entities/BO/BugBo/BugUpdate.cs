namespace Bugget.Entities.BO.BugBo;

public class BugUpdate
{
    public required int Id { get; set; }
    public required int ReportId { get; set; }
    public string? Receive { get; set; }
    public string? Expect { get; set; }
    public int? Status { get; set; }
    public required string UpdaterUserId { get; set; }
}
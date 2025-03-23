namespace Bugget.Entities.DTO.Report;

public sealed class ReportsSearchDto
{
    public string? Search { get; set; }
    public string? Order { get; init; } = "created";
    public bool? Desc { get; init; } = true;
    public int[]? ReportStatuses { get; init; }
    public string[]? UserIds { get; init; }
    public string[]? TeamIds { get; init; }
}
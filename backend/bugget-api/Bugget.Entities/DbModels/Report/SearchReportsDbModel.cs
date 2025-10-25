namespace Bugget.Entities.DbModels.Report;

public sealed class SearchReportsDbModel
{
    public ReportDbModel[] Reports { get; set; } = [];
    public int Total { get; set; }
}
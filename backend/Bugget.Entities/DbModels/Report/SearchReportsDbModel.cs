namespace Bugget.Entities.DbModels.Report;

public sealed class SearchReportsDbModel
{
    public ReportObsoleteDbModel[] Reports { get; set; } = [];
    public int Total { get; set; }
}
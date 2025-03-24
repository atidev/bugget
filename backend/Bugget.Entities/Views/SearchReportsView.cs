namespace Bugget.Entities.Views;

public sealed class SearchReportsView
{
    public ReportView[] Reports { get; init; }
    public int Total { get; init; }
}
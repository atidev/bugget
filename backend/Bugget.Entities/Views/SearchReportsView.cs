namespace Bugget.Entities.Views;

public sealed class SearchReportsView<TReportView>
{
    public TReportView[] Reports { get; init; }
    public int Total { get; init; }
}
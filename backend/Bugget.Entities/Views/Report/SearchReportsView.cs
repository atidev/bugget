namespace Bugget.Entities.Views;

public sealed class SearchReportsView<TReportView>
{
    public required TReportView[] Reports { get; init; }
    public required int Total { get; init; }
}
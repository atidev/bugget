using Bugget.Entities.DbModels.Report;

namespace Bugget.Entities.Views;

public sealed class ReportViews
{
    public required long Total { get; init; }
    public required ReportDbModel[] Reports { get; init; }
}
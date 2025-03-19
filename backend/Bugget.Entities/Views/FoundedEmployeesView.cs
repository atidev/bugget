namespace Bugget.Entities.Views;

public class FoundedEmployeesView
{
    public required IEnumerable<EmployeeView> Employees { get; init; }
    public required int Total { get; init; }
}
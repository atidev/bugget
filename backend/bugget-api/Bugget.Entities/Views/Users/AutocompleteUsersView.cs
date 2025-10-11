namespace Bugget.Entities.Views.Users;

public class AutocompleteUsersView
{
    public required IEnumerable<UserView> Users { get; init; }
    public required int Total { get; init; }
}
using Bugget.Entities.BO;
using Bugget.Entities.Views;

namespace Bugget.DA.Interfaces;

public interface IUsersClient
{
    Task<User?> GetUserAsync(string userId);
    Task<IEnumerable<User>> GetUsersAsync(IEnumerable<string> userIds, string? organizationId);
    
    [Obsolete]
    IReadOnlyDictionary<string, User> DictUsers();

    [Obsolete]
    public IReadOnlyDictionary<string, IReadOnlyCollection<User>> DictUsersByTeam();
    
    public Task<(IEnumerable<User>, int)> AutocompleteUsersAsync(
        string userId,
        string searchString,
        int skip,
        int take,
        uint depth);
}
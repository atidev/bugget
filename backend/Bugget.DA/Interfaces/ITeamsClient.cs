using Bugget.Entities.BO;

namespace Bugget.DA.Interfaces;

public interface ITeamsClient
{
    public (IEnumerable<Team>, int) AutocompleteTeams(
        string userTeamId,
        string searchString,
        int skip,
        int take,
        uint depth);
} 
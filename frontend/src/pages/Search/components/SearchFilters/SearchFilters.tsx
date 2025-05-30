import { employeesAutocomplete } from "@/apiObsolete/employees";
import { teamsAutocomplete } from "@/apiObsolete/teams";
import Autosuggest from "@/components/Autosuggest/Autosuggest";
import Dropdown from "@/components/Dropdown/Dropdown";
import {
  updateStatuses,
  $statuses,
  $userFilter,
  updateUserFilter,
  $teamFilter,
  updateTeamFilter,
} from "@/storeObsolete/search";
import { User } from "@/types/user";
import { Team } from "@/typesObsolete/team";
import { useUnit } from "effector-react";

const autocompleteUsers = async (searchString: string) => {
  const response = await employeesAutocomplete(searchString);
  return (response.employees ?? []).map((employee: User) => ({
    id: employee.id,
    display: employee.name,
  }));
};

const autocompleteTeams = async (searchString: string) => {
  const response = await teamsAutocomplete(searchString);
  return (response.teams ?? []).map((team: Team) => ({
    id: team.id,
    display: team.name,
  }));
};

const SearchFilters = () => {
  const [statuses, userFilter, teamFilter] = useUnit([
    $statuses,
    $userFilter,
    $teamFilter,
  ]);
  return (
    <div className="flex flex-col gap-3">
      <div className="mb-4 text-lg font-base font-medium">
        Поисковые фильтры
      </div>
      <Dropdown
        onResetValue={null}
        label="Статус репорта"
        multiple
        value={statuses}
        onChange={(value) => {
          const list = Array.isArray(value) ? value : [value];
          updateStatuses(list.filter((v) => v !== null));
        }}
        options={[
          { label: "В работе", value: 0 },
          { label: "Решён", value: 1 },
        ]}
      />
      <div>
        <div className="mb-1 text-xs font-semibold">Участник</div>
        <Autosuggest
          onSelect={(entity) =>
            updateUserFilter(
              entity ? { id: entity.id, name: entity.display } : null
            )
          }
          externalString={userFilter?.name || ""}
          autocompleteFn={autocompleteUsers}
        />
      </div>
      <div>
        <div className="mb-1 text-xs font-semibold">Команда</div>
        <Autosuggest
          onSelect={(entity) =>
            updateTeamFilter(
              entity ? { id: entity.id, name: entity.display } : null
            )
          }
          externalString={teamFilter?.name || ""}
          autocompleteFn={autocompleteTeams}
        />
      </div>
    </div>
  );
};

export default SearchFilters;

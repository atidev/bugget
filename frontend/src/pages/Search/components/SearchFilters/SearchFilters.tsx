import { autocompleteUsers } from "@/api/users";
import { teamsAutocomplete } from "@/api/teams";
import Autosuggest from "@/components/Autosuggest/Autosuggest";
import Dropdown from "@/components/Dropdown/Dropdown";
import {
  updateStatuses,
  $statuses,
  $userFilter,
  updateUserFilter,
  $teamFilter,
  updateTeamFilter,
} from "@/store/search";
import { UserResponse } from "@/types/user";
import { Team } from "@/types/team";
import { useUnit } from "effector-react";
import { ReportStatuses } from "@/const";

const autocompleteUsersHandler = async (searchString: string) => {
  const response = await autocompleteUsers(searchString);
  return (response.users ?? []).map((user: UserResponse) => ({
    id: user.id,
    display: user.name,
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
          { label: "В работе", value: ReportStatuses.IN_PROGRESS },
          { label: "Решён", value: ReportStatuses.RESOLVED },
          { label: "Отклонён", value: ReportStatuses.REJECTED },
          { label: "Бэклог", value: ReportStatuses.BACKLOG },
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
          autocompleteFn={autocompleteUsersHandler}
          width={72}
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
          width={72}
        />
      </div>
    </div>
  );
};

export default SearchFilters;

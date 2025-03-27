import { employeesAutocomplete } from "@/api/users";
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
import { User } from "@/types/user";
import { Team } from "@/types/team";
import { useUnit } from "effector-react";

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
            updateUserFilter({ id: entity.id, name: entity.display })
          }
          externalString={userFilter?.name}
          autocompleteFn={async (searchString) => {
            const response = await employeesAutocomplete(searchString);
            return (response.employees ?? []).map((e: User) => ({
              id: e.id,
              display: e.name,
            }));
          }}
        />
      </div>
      <div>
        <div className="mb-1 text-xs font-semibold">Команда</div>
        <Autosuggest
          onSelect={(entity) =>
            updateTeamFilter({ id: entity.id, name: entity.display })
          }
          externalString={teamFilter?.name}
          autocompleteFn={async (searchString) => {
            const response = await teamsAutocomplete(searchString);
            return (response.teams ?? []).map((t: Team) => ({
              id: t.id,
              display: t.name,
            }));
          }}
        />
      </div>
    </div>
  );
};

export default SearchFilters;

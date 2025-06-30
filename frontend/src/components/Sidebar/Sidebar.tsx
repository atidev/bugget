import Autosuggest from "@/components/Autosuggest/Autosuggest";
import Avatar from "@/components/Avatar/Avatar";
import { autocompleteUsers } from "@/api/employees";
import { UserResponse } from "@/types/user";
import { useUnit } from "effector-react";
import {
  $responsibleUserIdStore,
  $responsibleUserNameStore,
  $participantsWithNamesStore,
  changeResponsibleUserIdEvent,
} from "@/store/report";
import { StatusSelect } from "./StatusSelect";

const autocompleteUsersHandler = async (searchString: string) => {
  const response = await autocompleteUsers(searchString);
  return (response.employees ?? []).map((employee: UserResponse) => ({
    id: employee.id,
    display: employee.name,
  }));
};

const Sidebar = () => {
  const responsibleUserId = useUnit($responsibleUserIdStore);
  const responsibleUserName = useUnit($responsibleUserNameStore);
  const participantsWithNames = useUnit($participantsWithNamesStore);

  return (
    <div className="flex flex-col gap-4 px-6 py-8">
      <div className="flex flex-col gap-2">
        <div className="text-sm text-base-content/70">Статус</div>
        <StatusSelect />
      </div>

      <div className="flex flex-col gap-2">
        <div className="text-sm text-base-content/70">Ответственный</div>
        <Autosuggest
          onSelect={(entity) => changeResponsibleUserIdEvent(entity?.id || "")}
          externalString={responsibleUserName || responsibleUserId}
          autocompleteFn={autocompleteUsersHandler}
        />
      </div>

      <div className="flex flex-col gap-2">
        <div className="text-sm text-base-content/70">Участники</div>
        <div className="flex gap-2 mt-2">
          {participantsWithNames?.map((participant) => (
            <div
              key={participant.id}
              className="tooltip tooltip-bottom"
              data-tip={participant.name}
            >
              <Avatar />
            </div>
          ))}
          {(!participantsWithNames || participantsWithNames.length === 0) && (
            <span className="text-sm text-base-content/50">Нет участников</span>
          )}
        </div>
      </div>
    </div>
  );
};

export default Sidebar;

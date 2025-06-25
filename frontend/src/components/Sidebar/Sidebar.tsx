import { ReportStatuses } from "@/const";
import Autosuggest from "@/components/Autosuggest/Autosuggest";
import Avatar from "@/components/Avatar/Avatar";
import { autocompleteUsers } from "@/api/employees";
import { UserResponse } from "@/types/user";
// import { $user } from "@/store/user";
import { useUnit } from "effector-react";
import {
  $statusStore,
  $responsibleUserIdStore,
  $responsibleUserNameStore,
  $participantsWithNamesStore,
  changeStatusEvent,
  changeResponsibleUserIdEvent,
} from "@/store/report";

const autocompleteUsersHandler = async (searchString: string) => {
  const response = await autocompleteUsers(searchString);
  return (response.employees ?? []).map((employee: UserResponse) => ({
    id: employee.id,
    display: employee.name,
  }));
};

const Sidebar = () => {
  const status = useUnit($statusStore);
  const responsibleUserId = useUnit($responsibleUserIdStore);
  const responsibleUserName = useUnit($responsibleUserNameStore);
  const participantsWithNames = useUnit($participantsWithNamesStore);

  return (
    <div className="flex flex-col gap-4 px-6 py-8 border-l border-base-content/70 rounded-sm">
      <div className="flex flex-col gap-2">
        <div className="text-sm text-base-content/70">Статус</div>
        <div className="flex flex-row items-center gap-2">
          <div
            aria-label="status"
            className="status status-neutral status-xl"
          ></div>
          <select
            value={status}
            onChange={(e) =>
              changeStatusEvent(Number(e.target.value) as ReportStatuses)
            }
            className="appearance-none"
          >
            <option value={ReportStatuses.BACKLOG}>Backlog</option>
            <option value={ReportStatuses.RESOLVED}>Resolved</option>
            <option value={ReportStatuses.IN_PROGRESS}>In Progress</option>
            <option value={ReportStatuses.REJECTED}>Rejected</option>
          </select>
        </div>
        <div className="flex flex-row gap-2">
          <div className="flex flex-col gap-1">
            <Autosuggest
              onSelect={(entity) =>
                changeResponsibleUserIdEvent(entity?.id || "")
              }
              externalString={responsibleUserName || responsibleUserId}
              autocompleteFn={autocompleteUsersHandler}
            />
          </div>
        </div>
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

import Autosuggest from "@/components/Autosuggest/Autosuggest";
import Avatar from "@/components/Avatar/Avatar";
import { autocompleteUsers } from "@/api/users";
import { UserResponse } from "@/types/user";
import { useUnit } from "effector-react";
import {
  $responsibleUserNameStore,
  $participantsWithNamesStore,
  changeResponsibleUserIdEvent,
} from "@/store/report";
import ReportStatusSelect from "./components/ReportStatusSelect";
import SidebarContainer from "./SidebarContainer";

const autocompleteUsersHandler = async (searchString: string) => {
  const response = await autocompleteUsers(searchString);
  return (response.users ?? []).map((user: UserResponse) => ({
    id: user.id,
    display: user.name,
  }));
};

const RightSidebar = () => {
  const responsibleUserName = useUnit($responsibleUserNameStore);
  const participantsWithNames = useUnit($participantsWithNamesStore);

  return (
    <SidebarContainer>
      <div className="flex flex-col gap-2">
        <div className="text-sm text-base-content/70">Статус</div>
        <ReportStatusSelect />
      </div>

      <div className="flex flex-col gap-2">
        <div className="text-sm text-base-content/70">Ответственный</div>
        <Autosuggest
          onSelect={(entity) => changeResponsibleUserIdEvent(entity?.id || "")}
          externalString={responsibleUserName}
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
    </SidebarContainer>
  );
};

export default RightSidebar;

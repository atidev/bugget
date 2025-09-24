import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";

import { useUnit } from "effector-react";
import { BugStatuses } from "@/const";
import { BugClientEntity } from "@/types/bug";
import { $user } from "@/store/user";
import { $usersStore } from "@/store/report";

import BugStatusSelect from "./components/BugStatusSelect/BugStatusSelect";

type Props = {
  bug: BugClientEntity;
  onStatusChange?: (status: BugStatuses) => void;
};

const BugHeader = ({ bug, onStatusChange }: Props) => {
  const currentUser = useUnit($user);
  const users = useUnit($usersStore);
  const isAuthorCurrentUser = Boolean(
    currentUser?.id && bug.creatorUserId === currentUser.id
  );
  const authorName = users[bug.creatorUserId]?.name;
  const authorFragment = isAuthorCurrentUser
    ? " вами"
    : authorName
    ? ` пользователем ${authorName}`
    : "";
  return (
    <div className="col-span-2 flex justify-between items-center mb-4">
      <div className="flex items-center gap-2">
        <span className="text-lg font-bold min-h-[28px] flex items-center">
          {bug.isLocalOnly ? "Новый баг" : `Баг #${bug.id}`}
        </span>
        {!bug.isLocalOnly && bug.createdAt && (
          <span className="text-sm text-base-content/60">
            Создан{" "}
            {formatDistanceToNow(new Date(bug.createdAt), {
              addSuffix: true,
              locale: ru,
            })}
            <span>{authorFragment}</span>
          </span>
        )}
      </div>
      {!bug.isLocalOnly && onStatusChange && (
        <BugStatusSelect status={bug.status} onChange={onStatusChange} />
      )}
    </div>
  );
};

export default BugHeader;

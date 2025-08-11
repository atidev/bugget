import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";

import { BugStatuses } from "@/const";
import { BugClientEntity } from "@/types/bug";

import BugStatusSelect from "./components/BugStatusSelect/BugStatusSelect";

type Props = {
  bug: BugClientEntity;
  onStatusChange?: (status: BugStatuses) => void;
};

const BugHeader = ({ bug, onStatusChange }: Props) => {
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

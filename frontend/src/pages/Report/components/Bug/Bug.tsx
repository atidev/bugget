import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import BugStatusSelect from "./components/BugStatusSelect/BugStatusSelect";
import Result from "./components/Result/Result";

import { BugStatuses } from "@/const";
import { BugEntity } from "@/types/bug";
import { changeBugStatusEvent } from "@/store/bugs";
import { useBugActions } from "@/hooks/useBugActions";

type Props = {
  bug: BugEntity;
};

const Bug = ({ bug }: Props) => {
  const { updateReceive, updateExpect } = useBugActions();

  const handleReceiveChange = (newReceive: string) => {
    updateReceive(bug.id, newReceive);
  };

  const handleExpectChange = (newExpect: string) => {
    updateExpect(bug.id, newExpect);
  };

  const handleStatusChange = (status: BugStatuses) => {
    changeBugStatusEvent({ bugId: bug.id, status });
  };

  return (
    <div
      className={`card bg-base-100 shadow-lg border border-base-300 mb-4 p-4 grid grid-cols-2 gap-4 ${
        bug.status === BugStatuses.ARCHIVED ? "border-success" : ""
      }`}
    >
      <div className="col-span-2 flex justify-between items-center mb-4">
        <div className="flex items-center gap-2">
          <span id={bug.id?.toString()} className="text-lg font-bold">
            {!bug.id ? "Новый баг" : `Баг #${bug.id}`}
          </span>
          {bug.id && bug.createdAt && (
            <span className="text-sm text-base-content/60">
              Создан{" "}
              {formatDistanceToNow(new Date(bug.createdAt), {
                addSuffix: true,
                locale: ru,
              })}
            </span>
          )}
        </div>
        {bug.id && (
          <BugStatusSelect status={bug.status} onChange={handleStatusChange} />
        )}
      </div>

      <Result
        title="🔴 фактический результат"
        value={bug.receive || ""}
        onSave={handleReceiveChange}
        colorType="error"
      />

      <Result
        title="🟢 ожидаемый результат"
        value={bug.expect || ""}
        onSave={handleExpectChange}
        colorType="success"
      />
    </div>
  );
};

export default Bug;

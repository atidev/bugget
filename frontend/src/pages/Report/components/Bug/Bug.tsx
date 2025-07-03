import { formatDistanceToNow } from "date-fns";
import { ru } from "date-fns/locale";
import BugStatusSelect from "./components/BugStatusSelect/BugStatusSelect";
import Result from "./components/Result/Result";

import { BugStatuses } from "@/const";
import { BugEntity, BugFormData } from "@/types/bug";
import {
  changeBugStatusEvent,
  patchBugSocketEvent,
  updateBugDataEvent,
} from "@/store/bugs";
import { useUnit } from "effector-react";
import { $reportIdStore } from "@/store/report";
import { CircleSmall } from "lucide-react";
import { SocketEvent } from "@/webSocketApi/models";
import { useSocketEvent } from "@/hooks/useSocketEvent";

type Props = {
  bug: BugEntity;
};

const Bug = ({ bug }: Props) => {
  const reportId = useUnit($reportIdStore);

  useSocketEvent(SocketEvent.BugPatch, (patch) => {
    patchBugSocketEvent({ bugId: patch.bugId, patch: patch.patch });
  });

  const updateBugFields = (bugId: number, data: Partial<BugFormData>) => {
    if (!reportId) return;
    updateBugDataEvent({ bugId, reportId, data });
  };

  const handleReceiveChange = (newReceive: string) => {
    updateBugFields(bug.id, { receive: newReceive });
  };

  const handleExpectChange = (newExpect: string) => {
    updateBugFields(bug.id, { expect: newExpect });
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
        title={
          <span className="inline-flex items-center">
            <CircleSmall
              size={20}
              color="var(--color-error)"
              fill="var(--color-error)"
            />{" "}
            фактический результат
          </span>
        }
        value={bug.receive || ""}
        onSave={handleReceiveChange}
        colorType="error"
      />

      <Result
        title={
          <span className="inline-flex items-center">
            <CircleSmall
              size={20}
              color="var(--color-success)"
              fill="var(--color-success)"
            />{" "}
            ожидаемый результат
          </span>
        }
        value={bug.expect || ""}
        onSave={handleExpectChange}
        colorType="success"
      />
    </div>
  );
};

export default Bug;

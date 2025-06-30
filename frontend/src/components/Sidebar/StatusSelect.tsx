import { useUnit } from "effector-react";
import { ReportStatuses } from "@/const";
import { $statusStore, changeStatusEvent } from "@/store/report";
import { StatusIndicator } from "@/components/StatusIndicator/StatusIndicator";

import { Status } from "@/components/StatusIndicator/StatusIndicator";

const reportToIndicator: Record<ReportStatuses, Status> = {
  [ReportStatuses.BACKLOG]: "backlog",
  [ReportStatuses.IN_PROGRESS]: "inProgress",
  [ReportStatuses.RESOLVED]: "done",
  [ReportStatuses.REJECTED]: "rejected",
};

const ORDER: ReportStatuses[] = [
  ReportStatuses.BACKLOG,
  ReportStatuses.IN_PROGRESS,
  ReportStatuses.RESOLVED,
  ReportStatuses.REJECTED,
];

export const StatusSelect = () => {
  const status = useUnit($statusStore);

  return (
    <div className="dropdown">
      <label
        tabIndex={0}
        className="flex items-center gap-2 justify-start w-full cursor-pointer"
      >
        <StatusIndicator status={reportToIndicator[status]} />
      </label>

      <ul
        tabIndex={0}
        className="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-full mt-1 z-10"
      >
        {ORDER.map((rs) => (
          <li key={rs} onClick={() => changeStatusEvent(rs)}>
            <a className="flex items-center gap-2">
              <StatusIndicator status={reportToIndicator[rs]} />
            </a>
          </li>
        ))}
      </ul>
    </div>
  );
};

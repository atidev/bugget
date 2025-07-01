import { useUnit } from "effector-react";
import { ReportStatuses } from "@/const";
import { $statusStore, changeStatusEvent } from "@/store/report";
import StatusIndicator from "./StatusIndicator";

const optionsOrder: ReportStatuses[] = [
  ReportStatuses.BACKLOG,
  ReportStatuses.IN_PROGRESS,
  ReportStatuses.RESOLVED,
  ReportStatuses.REJECTED,
];

const StatusSelect = () => {
  const status = useUnit($statusStore);

  return (
    <div className="dropdown">
      <label
        tabIndex={0}
        className="flex items-center gap-2 justify-start w-full cursor-pointer"
      >
        <StatusIndicator status={status} />
      </label>

      <ul
        tabIndex={0}
        className="dropdown-content menu p-2 shadow bg-base-100 rounded-box w-full mt-1 z-10"
      >
        {optionsOrder.map((option) => (
          <li key={option} onClick={() => changeStatusEvent(option)}>
            <a className="flex items-center gap-2">
              <StatusIndicator status={option} />
            </a>
          </li>
        ))}
      </ul>
    </div>
  );
};
export default StatusSelect;

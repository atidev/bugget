import { useUnit } from "effector-react";
import { ReportStatuses, reportStatusMap } from "@/const";
import { $statusStore, changeStatusEvent } from "@/store/report";
import StatusIndicator from "@/components/StatusIndicator/StatusIndicator";
import StatusSelect from "@/components/StatusSelect/StatusSelect";

const reportStatusOptions = [
  {
    value: ReportStatuses.BACKLOG,
    indicator: (
      <StatusIndicator statusMeta={reportStatusMap[ReportStatuses.BACKLOG]} />
    ),
  },
  {
    value: ReportStatuses.IN_PROGRESS,
    indicator: (
      <StatusIndicator
        statusMeta={reportStatusMap[ReportStatuses.IN_PROGRESS]}
      />
    ),
  },
  {
    value: ReportStatuses.RESOLVED,
    indicator: (
      <StatusIndicator statusMeta={reportStatusMap[ReportStatuses.RESOLVED]} />
    ),
  },
  {
    value: ReportStatuses.REJECTED,
    indicator: (
      <StatusIndicator statusMeta={reportStatusMap[ReportStatuses.REJECTED]} />
    ),
  },
];

const ReportStatusSelect = () => {
  const status = useUnit($statusStore);

  return (
    <StatusSelect
      status={status}
      options={reportStatusOptions}
      onChange={changeStatusEvent}
      className="w-full"
    />
  );
};

export default ReportStatusSelect;

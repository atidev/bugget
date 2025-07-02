import { useUnit } from "effector-react";
import { ReportStatuses } from "@/const";
import { $statusStore, changeStatusEvent } from "@/store/report";
import StatusIndicator, {
  StatusConfig,
} from "@/components/StatusIndicator/StatusIndicator";
import StatusSelect from "@/components/StatusSelect/StatusSelect";
import { CheckCircle, Clock, XCircle } from "lucide-react";
import { CircleDashed } from "lucide-react";

const reportStatusConfig: StatusConfig<ReportStatuses> = {
  [ReportStatuses.BACKLOG]: {
    label: "Бэклог",
    icon: CircleDashed,
    iconColor: "text-base-content",
  },
  [ReportStatuses.IN_PROGRESS]: {
    label: "В работе",
    icon: Clock,
    iconColor: "text-warning",
  },
  [ReportStatuses.RESOLVED]: {
    label: "Готово",
    icon: CheckCircle,
    iconColor: "text-success",
  },
  [ReportStatuses.REJECTED]: {
    label: "Отменён",
    icon: XCircle,
    iconColor: "text-error",
  },
};

const reportStatusOptions = [
  {
    value: ReportStatuses.BACKLOG,
    indicator: (
      <StatusIndicator
        status={ReportStatuses.BACKLOG}
        config={reportStatusConfig}
      />
    ),
  },
  {
    value: ReportStatuses.IN_PROGRESS,
    indicator: (
      <StatusIndicator
        status={ReportStatuses.IN_PROGRESS}
        config={reportStatusConfig}
      />
    ),
  },
  {
    value: ReportStatuses.RESOLVED,
    indicator: (
      <StatusIndicator
        status={ReportStatuses.RESOLVED}
        config={reportStatusConfig}
      />
    ),
  },
  {
    value: ReportStatuses.REJECTED,
    indicator: (
      <StatusIndicator
        status={ReportStatuses.REJECTED}
        config={reportStatusConfig}
      />
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

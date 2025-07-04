import { BugStatuses } from "@/const";
import StatusIndicator from "@/components/StatusIndicator/StatusIndicator";
import StatusSelect from "@/components/StatusSelect/StatusSelect";
import { CircleX, CircleCheck, XCircle } from "lucide-react";

type Props = {
  status: BugStatuses;
  onChange: (status: BugStatuses) => void;
  className?: string;
};

const bugStatusConfig = {
  [BugStatuses.ACTIVE]: {
    label: "Открыт",
    icon: CircleX,
    iconColor: "text-error",
  },
  [BugStatuses.ARCHIVED]: {
    label: "Исправлен",
    icon: CircleCheck,
    iconColor: "text-success",
  },
  [BugStatuses.REJECTED]: {
    label: "Отклонен",
    icon: XCircle,
    iconColor: "text-warning",
  },
};

const BugStatusSelect = ({ status, onChange, className }: Props) => {
  const bugStatusOptions = [
    {
      value: BugStatuses.ACTIVE,
      indicator: (
        <StatusIndicator status={BugStatuses.ACTIVE} config={bugStatusConfig} />
      ),
    },
    {
      value: BugStatuses.ARCHIVED,
      indicator: (
        <StatusIndicator
          status={BugStatuses.ARCHIVED}
          config={bugStatusConfig}
        />
      ),
    },
    {
      value: BugStatuses.REJECTED,
      indicator: (
        <StatusIndicator
          status={BugStatuses.REJECTED}
          config={bugStatusConfig}
        />
      ),
    },
  ];

  return (
    <StatusSelect
      status={status}
      options={bugStatusOptions}
      onChange={onChange}
      className={className}
    />
  );
};

export default BugStatusSelect;

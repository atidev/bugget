import { CircleDashed, Clock, CheckCircle, XCircle } from "lucide-react";
import { ReportStatuses } from "@/const";

type Props = {
  status: ReportStatuses;
};

const statusConfig: Record<
  ReportStatuses,
  {
    label: string;
    icon: React.ElementType;
    iconColor: string; // цвет для иконки
  }
> = {
  [ReportStatuses.BACKLOG]: {
    label: "Backlog",
    icon: CircleDashed,
    iconColor: "text-base-content",
  },
  [ReportStatuses.IN_PROGRESS]: {
    label: "In Progress",
    icon: Clock,
    iconColor: "text-warning",
  },
  [ReportStatuses.RESOLVED]: {
    label: "Done",
    icon: CheckCircle,
    iconColor: "text-success",
  },
  [ReportStatuses.REJECTED]: {
    label: "Rejected",
    icon: XCircle,
    iconColor: "text-error",
  },
};

const StatusIndicator = ({ status }: Props) => {
  const { label, icon: Icon, iconColor } = statusConfig[status];

  return (
    <div className="flex items-center space-x-2">
      <Icon className={`w-4 h-4 ${iconColor}`} />
      <span className="text-sm">{label}</span>
    </div>
  );
};

export default StatusIndicator;

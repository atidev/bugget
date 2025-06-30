import { CircleDashed, Clock, CheckCircle, XCircle } from "lucide-react";

export type Status = "backlog" | "inProgress" | "done" | "rejected";

type Props = {
  status: Status;
};

const statusConfig: Record<
  Status,
  {
    label: string;
    icon: React.ElementType;
    iconColor: string; // цвет для иконки
  }
> = {
  backlog: {
    label: "Backlog",
    icon: CircleDashed,
    iconColor: "text-base-content",
  },
  inProgress: {
    label: "In Progress",
    icon: Clock,
    iconColor: "text-warning",
  },
  done: {
    label: "Done",
    icon: CheckCircle,
    iconColor: "text-success",
  },
  rejected: {
    label: "Rejected",
    icon: XCircle,
    iconColor: "text-error",
  },
};

export const StatusIndicator = ({ status }: Props) => {
  const { label, icon: Icon, iconColor } = statusConfig[status];

  return (
    <div className="flex items-center space-x-2">
      <Icon className={`w-4 h-4 ${iconColor}`} />
      <span className="text-sm">{label}</span>
    </div>
  );
};
